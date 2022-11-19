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
    internal class StopCommand : ICommand, IUsageProvider
    {
        /// <summary>
        /// The command permission.
        /// </summary>
        public const string Permission = "vcm.stop";

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static StopCommand Instance { get; } = new StopCommand();

        /// <inheritdoc/>
        public string Command { get; } = "stop";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "s", "st" };

        /// <inheritdoc/>
        public string Description { get; } = VoiceChatManager.Instance.Translation.StopCommandDescription;

        /// <inheritdoc/>
        public string[] Usage { get; } = { VoiceChatManager.Instance.Translation.StopCommandUsage };

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = Usage[0];
                return false;
            }

            if (!sender.CheckPermission(Permission))
            {
                response = string.Format(VoiceChatManager.Instance.Translation.NotEnoughPermissionsError, Permission);
                return false;
            }

            if ((arguments.Count == 2 && arguments.At(0).TryStop(arguments.At(1).GetChannelName(), out var _)) ||
                (int.TryParse(arguments.At(0), out var id) && id.TryStop(out _)))
            {
                response = string.Format(VoiceChatManager.Instance.Translation.AudioHasBeenStopped, arguments.At(0));
                return true;
            }

            response = string.Format(VoiceChatManager.Instance.Translation.AudioNotFoundOrAlreadyStopped, arguments.At(0));
            return false;
        }
    }
}
