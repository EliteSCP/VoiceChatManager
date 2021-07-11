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
        /// <summary>
        /// The command permission.
        /// </summary>
        public const string Permission = "vcm.list.clear";

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
        public string Description { get; } = VoiceChatManager.Instance.Translation.ClearCommandDescription;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(Permission))
            {
                response = string.Format(VoiceChatManager.Instance.Translation.NotEnoughPermissionsError, Permission);
                return false;
            }

            foreach (var streamedMicrophone in StreamedMicrophone.List)
                streamedMicrophone.Dispose();

            StreamedMicrophone.List.Clear();

            response = VoiceChatManager.Instance.Translation.AudiosListClearedSuccess;
            return true;
        }
    }
}
