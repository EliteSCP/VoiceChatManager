// -----------------------------------------------------------------------
// <copyright file="MainCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands
{
    using System;
    using CommandSystem;
    using List;

    /// <summary>
    /// The main command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    internal class MainCommand : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainCommand"/> class.
        /// </summary>
        public MainCommand() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command { get; } = "voicechatmanager";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "vcm" };

        /// <inheritdoc/>
        public override string Description { get; } = string.Empty;

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(PlayCommand.Instance);
            RegisterCommand(StopCommand.Instance);
            RegisterCommand(PauseCommand.Instance);
            RegisterCommand(ListCommand.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand! Available: play, pause, stop, list";
            return false;
        }
    }
}
