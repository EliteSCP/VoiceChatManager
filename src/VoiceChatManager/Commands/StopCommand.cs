// -----------------------------------------------------------------------
// <copyright file="StopCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands
{
    using System;
    using CommandSystem;
    using Core.Extensions;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Stop an audio file from playing.
    /// </summary>
    internal class StopCommand : ICommand
    {
        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static StopCommand Instance { get; } = new StopCommand();

        /// <inheritdoc/>
        public string Command { get; } = "stop";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "s", "st" };

        /// <inheritdoc/>
        public string Description { get; } = "Stops an audio file from playing.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "voicechatmanager stop [Preset name/File name/File path/Audio ID]";
                return false;
            }

            if (!sender.CheckPermission("vcm.stop"))
            {
                response = "Not enough permissions to run this command!\nRequired: vcm.stop";
                return false;
            }

            if (arguments.At(0).TryStop(out var _) || (int.TryParse(arguments.At(0), out var id) && id.TryStop(out _)))
            {
                response = $"Audio \"{arguments.At(0)}\" has been stopped.";
                return true;
            }

            response = $"Audio \"{arguments.At(0)}\" not found or already stopped!";
            return false;
        }
    }
}
