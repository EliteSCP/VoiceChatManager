// -----------------------------------------------------------------------
// <copyright file="PlayCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using CommandSystem;
    using Core.Audio.Capture;
    using Core.Extensions;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;

    /// <summary>
    /// Plays an audio file to everyone in the server.
    /// </summary>
    internal class PlayCommand : ICommand
    {
        /// <summary>
        /// The extension of converted files through this command.
        /// </summary>
        public const string ConvertedFileExtension = ".f32le";

        private PlayCommand()
        {
        }

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static PlayCommand Instance { get; } = new PlayCommand();

        /// <inheritdoc/>
        public string Command { get; } = "play";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "p", "pl" };

        /// <inheritdoc/>
        public string Description { get; } = "Plays an audio file on a specific channel.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2 || arguments.Count > 6 || arguments.Count == 5)
            {
                response = "\nvoicechatmanager play [File alias/File path] [Volume (0-100)]" +
                    "\nvoicechatmanager play [File alias/File path] [Volume (0-100)] [Channel name (SCP, Intercom, Proximity, Ghost)]" +
                    "\nvoicechatmanager play [File alias/File path] [Volume (0-100)] proximity [Player ID/Player Name/Player]" +
                    "\nvoicechatmanager play [File alias/File path] [Volume (0-100)] proximity [X] [Y] [Z]";
                return false;
            }

            if (!sender.CheckPermission("vcm.play"))
            {
                response = "Not enough permissions to run this command!\nRequired: vcm.play";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out var volume))
            {
                response = $"{arguments.At(1)} is not a valid volume, range varies from 0 to 100!";
                return false;
            }

            var channelName = arguments.Count == 2 ? "Intercom" : arguments.At(2).GetChannelName();

            if (!VoiceChatManager.Instance.Config.Presets.TryGetValue(arguments.At(0), out var path))
                path = arguments.At(0);

            var convertedFilePath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)) + ConvertedFileExtension;

            if (File.Exists(path) && !path.EndsWith(ConvertedFileExtension) && !File.Exists(convertedFilePath))
            {
                if (!VoiceChatManager.Instance.Config.IsFFmpegInstalled)
                {
                    response = $"Your FFmpeg directory isn't set up properly, \"{path}\" won't be converted and played.";
                    return false;
                }

                response = $"Converting \"{path}\"...";

                path.ConvertFileAsync().ContinueWith(
                    task =>
                {
                    if (task.IsCompleted)
                    {
                        var newArguments = new List<string>(arguments)
                        {
                            [0] = convertedFilePath,
                        };

                        var isSucceded = Execute(new ArraySegment<string>(newArguments.ToArray()), sender, out var otherResponse);

                        sender.Respond(otherResponse, isSucceded);
                    }
                    else
                    {
                        Log.Error($"Failed to convert \"{path}\": {task.Exception}");
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);

                return true;
            }

            if (int.TryParse(path, out var id) && id.TryPlay(volume, channelName, out var streamedMicrophone))
            {
                response = $"Playing \"{id}\" with {volume} volume on \"{streamedMicrophone.ChannelName}\" channel, duration: {streamedMicrophone.Duration.ToString(VoiceChatManager.Instance.Config.DurationFormat)}";
                return true;
            }

            if (!path.EndsWith(ConvertedFileExtension))
                path = convertedFilePath;

            if (arguments.Count == 2 || arguments.Count == 3)
            {
                if (path.TryPlay(volume, channelName, out streamedMicrophone))
                {
                    response = $"Playing \"{path}\" with {volume} volume on \"{streamedMicrophone.ChannelName}\" channel, duration: {streamedMicrophone.Duration.ToString(VoiceChatManager.Instance.Config.DurationFormat)}";
                    return true;
                }
            }
            else if (arguments.Count == 4)
            {
                if (!(Player.Get(arguments.At(3)) is Player player))
                {
                    response = $"Player \"{arguments.At(3)}\" not found!";
                    return false;
                }
                else if (path.TryPlay(Talker.GetOrCreate(player.GameObject), volume, channelName, out streamedMicrophone))
                {
                    response = $"Playing \"{path}\" with {volume} volume, in the proximity of \"{player.Nickname}\", duration: {streamedMicrophone.Duration.ToString(VoiceChatManager.Instance.Config.DurationFormat)}";
                    return true;
                }
            }
            else
            {
                if (!float.TryParse(arguments.At(3), out var x))
                {
                    response = $"\"{arguments.At(3)}\" is not a valid x coordinate!";
                    return false;
                }
                else if (!float.TryParse(arguments.At(4), out var y))
                {
                    response = $"\"{arguments.At(4)}\" is not a valid x coordinate!";
                    return false;
                }
                else if (!float.TryParse(arguments.At(5), out var z))
                {
                    response = $"\"{arguments.At(5)}\" is not a valid z coordinate!";
                    return false;
                }
                else if (path.TryPlay(new Vector3(x, y, z), volume, channelName, out streamedMicrophone))
                {
                    response = $"Playing \"{path}\" with {volume} volume, in the proximity of \"({x}, {y}, {z})\", duration: {streamedMicrophone.Duration.ToString(VoiceChatManager.Instance.Config.DurationFormat)}";
                    return true;
                }
            }

            response = $"Audio \"{path}\" not found or it's already playing!";
            return false;
        }
    }
}
