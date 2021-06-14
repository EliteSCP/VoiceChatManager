// -----------------------------------------------------------------------
// <copyright file="PresetsCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands.List
{
    using System;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using NorthwoodLib.Pools;

    /// <inheritdoc/>
    internal class PresetsCommand : ICommand
    {
        private PresetsCommand()
        {
        }

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static PresetsCommand Instance { get; } = new PresetsCommand();

        /// <inheritdoc/>
        public string Command { get; } = "presets";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "p", "pr", "pre" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets the list of audio presets.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 0)
            {
                response = "voicechatmanager list presets";
                return false;
            }

            if (!sender.CheckPermission("voicechatmanager.list.presets"))
            {
                response = "Not enough permissions to run this command!\nRequired: voicechatmanager.list.presets";
                return false;
            }

            var message = StringBuilderPool.Shared.Rent().AppendLine().Append("[Audio presets (").Append(VoiceChatManager.Instance.Config.Presets.Count).AppendLine(")]").AppendLine();

            if (VoiceChatManager.Instance.Config.Presets.Count > 0)
            {
                var i = 0;

                foreach (var preset in VoiceChatManager.Instance.Config.Presets)
                {
                    message.Append("[").Append(i++).Append(". ").Append(preset.Key).AppendLine("]").
                        Append("Path: ").AppendLine(preset.Value).AppendLine();
                }
            }
            else
            {
                message.Append("There are no audio presets.");
            }

            response = StringBuilderPool.Shared.ToStringReturn(message);
            return true;
        }
    }
}
