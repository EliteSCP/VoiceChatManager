// -----------------------------------------------------------------------
// <copyright file="PauseCommand.cs" company="iopietro">
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
    internal class PauseCommand : ICommand
    {
        /// <summary>
        /// The command permission.
        /// </summary>
        public const string Permission = "vcm.pause";

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static PauseCommand Instance { get; } = new PauseCommand();

        /// <inheritdoc/>
        public string Command { get; } = "pause";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "pa" };

        /// <inheritdoc/>
        public string Description { get; } = VoiceChatManager.Instance.Translation.PauseCommandDescription;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = VoiceChatManager.Instance.Translation.PauseCommandUsage;
                return false;
            }

            if (!sender.CheckPermission(Permission))
            {
                response = string.Format(VoiceChatManager.Instance.Translation.NotEnoughPermissionsError, Permission);
                return false;
            }

            if (arguments.At(0).TryPause(out var streamedMicrophone) || (int.TryParse(arguments.At(0), out var id) && id.TryPause(out streamedMicrophone)))
            {
                response = string.Format(VoiceChatManager.Instance.Translation.AudioHasBeenPaused, arguments.At(0), streamedMicrophone.Progression.ToString(VoiceChatManager.Instance.Config.DurationFormat), Math.Round(streamedMicrophone.Stream.Position / (float)streamedMicrophone.Stream.Length * 100f, 1));
                return true;
            }

            response = string.Format(VoiceChatManager.Instance.Translation.AudioNotFoundOrIsNotPlaying, arguments.At(0));
            return false;
        }
    }
}
