// -----------------------------------------------------------------------
// <copyright file="ListCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands.List
{
    using System;
    using CommandSystem;

    /// <summary>
    /// Lists all in-cache audios.
    /// </summary>
    internal class ListCommand : ParentCommand
    {
        private ListCommand() => LoadGeneratedCommands();

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static ListCommand Instance { get; } = new ListCommand();

        /// <inheritdoc/>
        public override string Command { get; } = "list";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "l", "li" };

        /// <inheritdoc/>
        public override string Description { get; } = string.Empty;

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(PresetsCommand.Instance);
            RegisterCommand(AudiosCommand.Instance);
            RegisterCommand(ClearCommand.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = string.Format(VoiceChatManager.Instance.Translation.InvalidSubCommandError, "presets, audios, clear");
            return true;
        }
    }
}
