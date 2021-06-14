// -----------------------------------------------------------------------
// <copyright file="PauseCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands
{
    using System;
    using Api.Extensions;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Stop an audio file from playing.
    /// </summary>
    internal class PauseCommand : ICommand
    {
        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static PauseCommand Instance { get; } = new PauseCommand();

        /// <inheritdoc/>
        public string Command { get; } = "pause";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "pa" };

        /// <inheritdoc/>
        public string Description { get; } = "Pause an audio file from playing.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "voicechatmanager pause [Preset name/File name/File path/Audio ID]";
                return false;
            }

            if (!sender.CheckPermission("voicechatmanager.pause"))
            {
                response = "Not enough permissions to run this command!\nRequired: voicechatmanager.pause";
                return false;
            }

            if (arguments.At(0).TryPause(out var streamedMicrophone) || (int.TryParse(arguments.At(0), out var id) && id.TryPause(out streamedMicrophone)))
            {
                response = $"Audio \"{arguments.At(0)}\" has been paused at {streamedMicrophone.Progression.ToString(VoiceChatManager.Instance.Config.DurationFormat)} ({streamedMicrophone.PercentageProgression}%).";
                return true;
            }

            response = $"Audio \"{arguments.At(0)}\" not found or it's not playing!";
            return false;
        }
    }
}
