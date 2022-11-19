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
        /// Gets the command permission.
        /// </summary>
        public const string Permission = "vcm.play";

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
        public string Description { get; } = VoiceChatManager.Instance.Translation.PlayCommandDescription;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 2 || arguments.Count > 6 || arguments.Count == 5)
            {
                response = VoiceChatManager.Instance.Translation.PlayCommandUsage;
                return false;
            }

            if (!sender.CheckPermission(Permission))
            {
                response = string.Format(VoiceChatManager.Instance.Translation.NotEnoughPermissionsError, Permission);
                return false;
            }

            if (!float.TryParse(arguments.At(1), out var volume))
            {
                response = string.Format(VoiceChatManager.Instance.Translation.InvalidVolumeError, arguments.At(1));
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
                    response = string.Format(VoiceChatManager.Instance.Translation.FFmpegDirectoryIsNotSetUpProperlyError, path);
                    return false;
                }

                response = string.Format(VoiceChatManager.Instance.Translation.ConvertingAudio, path);

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
                        Log.Error(string.Format(VoiceChatManager.Instance.Translation.FailedToConvert, path, task.Exception));
                    }
                },
                    TaskContinuationOptions.ExecuteSynchronously);

                return true;
            }

            if (int.TryParse(path, out var id) && id.TryPlay(volume, channelName, out var streamedMicrophone))
            {
                response = string.Format(
                    VoiceChatManager.Instance.Translation.AudioIsPlayingInAChannel, id, volume, streamedMicrophone.ChannelName, streamedMicrophone.Duration.ToString(VoiceChatManager.Instance.Config.DurationFormat));

                return true;
            }

            if (!path.EndsWith(ConvertedFileExtension))
                path = convertedFilePath;

            if (arguments.Count == 2 || arguments.Count == 3)
            {
                if (path.TryPlay(volume, channelName, out streamedMicrophone, log: VoiceChatManager.Instance.Log))
                {
                    response = string.Format(
                        VoiceChatManager.Instance.Translation.AudioIsPlayingInAChannel, path, volume, streamedMicrophone.ChannelName, streamedMicrophone.Duration.ToString(VoiceChatManager.Instance.Config.DurationFormat));

                    return true;
                }
            }
            else if (arguments.Count == 4)
            {
                if (!(Player.Get(arguments.At(3)) is Player player))
                {
                    response = string.Format(VoiceChatManager.Instance.Translation.PlayerNotFoundError, arguments.At(3));
                    return false;
                }
                else if (path.TryPlay(Talker.GetOrCreate(player.GameObject), volume, channelName, out streamedMicrophone, log: VoiceChatManager.Instance.Log))
                {
                    response = string.Format(
                        VoiceChatManager.Instance.Translation.AudioIsPlayingNearAPlayer, path, volume, player.Nickname, streamedMicrophone.Duration.ToString(VoiceChatManager.Instance.Config.DurationFormat));
                    return true;
                }
            }
            else
            {
                if (!float.TryParse(arguments.At(3), out var x))
                {
                    response = string.Format(VoiceChatManager.Instance.Translation.InvalidCoordinateError, arguments.At(3), "x");
                    return false;
                }
                else if (!float.TryParse(arguments.At(4), out var y))
                {
                    response = string.Format(VoiceChatManager.Instance.Translation.InvalidCoordinateError, arguments.At(4), "y");
                    return false;
                }
                else if (!float.TryParse(arguments.At(5), out var z))
                {
                    response = string.Format(VoiceChatManager.Instance.Translation.InvalidCoordinateError, arguments.At(5), "z");
                    return false;
                }
                else if (path.TryPlay(new Vector3(x, y, z), volume, channelName, out streamedMicrophone, log: VoiceChatManager.Instance.Log))
                {
                    response = string.Format(
                        VoiceChatManager.Instance.Translation.AudioIsPlayingInAPosition, path, volume, x, y, z, streamedMicrophone.Duration.ToString(VoiceChatManager.Instance.Config.DurationFormat));
                    return true;
                }
            }

            response = string.Format(VoiceChatManager.Instance.Translation.AudioNotFoundOrAlreadyPlaying, path);
            return false;
        }
    }
}
