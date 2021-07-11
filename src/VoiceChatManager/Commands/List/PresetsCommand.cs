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
        /// <summary>
        /// The command permission.
        /// </summary>
        public const string Permission = "vcm.list.presets";

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
        public string Description { get; } = VoiceChatManager.Instance.Translation.PresetCommandDescription;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(Permission))
            {
                response = string.Format(VoiceChatManager.Instance.Translation.NotEnoughPermissionsError, Permission);
                return false;
            }

            var message = StringBuilderPool.Shared.Rent().AppendLine()
                .Append("[").Append(VoiceChatManager.Instance.Translation.AudioPresets).Append(" (").Append(VoiceChatManager.Instance.Config.Presets.Count).AppendLine(")]")
                .AppendLine();

            if (VoiceChatManager.Instance.Config.Presets.Count > 0)
            {
                var i = 0;

                foreach (var preset in VoiceChatManager.Instance.Config.Presets)
                {
                    message.Append("[").Append(i++).Append(". ").Append(preset.Key).AppendLine("]").
                        Append(VoiceChatManager.Instance.Translation.Path).Append(": ").AppendLine(preset.Value).AppendLine();
                }
            }
            else
            {
                message.Append(VoiceChatManager.Instance.Translation.ThereAreNoAudioPresets);
            }

            response = StringBuilderPool.Shared.ToStringReturn(message);
            return true;
        }
    }
}
