// -----------------------------------------------------------------------
// <copyright file="ClearCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands.List
{
    using System;
    using CommandSystem;
    using Core.Audio.Playback;
    using Exiled.Permissions.Extensions;

    /// <inheritdoc/>
    internal class ClearCommand : ICommand
    {
        private ClearCommand()
        {
        }

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static ClearCommand Instance { get; } = new ClearCommand();

        /// <inheritdoc/>
        public string Command { get; } = "clear";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "cl", "c" };

        /// <inheritdoc/>
        public string Description { get; } = "Clears the audios list.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 0)
            {
                response = "voicechatmanager list clear";
                return false;
            }

            if (!sender.CheckPermission("voicechatmanager.list.clear"))
            {
                response = "Not enough permissions to run this command!\nRequired: voicechatmanager.list.clear";
                return false;
            }

            foreach (var streamedMicrophone in StreamedMicrophone.List)
                streamedMicrophone.Dispose();

            StreamedMicrophone.List.Clear();

            response = "Audios list cleared successfully!";
            return true;
        }
    }
}
