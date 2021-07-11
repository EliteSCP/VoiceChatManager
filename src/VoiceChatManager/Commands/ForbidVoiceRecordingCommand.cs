// -----------------------------------------------------------------------
// <copyright file="ForbidVoiceRecordingCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands
{
    using System;
    using CommandSystem;
    using Core.Audio.Capture;
    using Exiled.API.Features;

    /// <inheritdoc/>
    [CommandHandler(typeof(ClientCommandHandler))]
    internal class ForbidVoiceRecordingCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "forbidvoicerecording";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = VoiceChatManager.Instance.Translation.ForbidVoiceRecordingCommandDescription;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VoiceChatManager.Instance.Gdpr.IsCompliant || !VoiceChatManager.Instance.Config.Recorder.IsEnabled)
            {
                response = VoiceChatManager.Instance.Translation.CommandIsCurrentlyDisabled;
                return false;
            }

            if (!(Player.Get(sender as CommandSender) is Player player))
            {
                response = VoiceChatManager.Instance.Translation.ExecutingCommandError;
                return false;
            }

            if (!player.SessionVariables.TryGetValue("canBeVoiceRecorded", out object canBeVoiceRecorded) || !(bool)canBeVoiceRecorded)
            {
                response = VoiceChatManager.Instance.Translation.ForbidVoiceRecordingCommandWarning;
                return false;
            }

            player.SessionVariables.Remove("canBeVoiceRecorded");

            IVoiceChatRecorder voiceChatRecorder = null;
            ITalker talker = Talker.GetOrCreate(player.GameObject);

            if (talker.PlayBackComponent == null || (!VoiceChatManager.Instance.Capture?.Recorders.TryRemove(talker, out voiceChatRecorder) ?? true))
            {
                response = VoiceChatManager.Instance.Translation.YouCannotBeRemovedError;
                return true;
            }

            voiceChatRecorder?.Dispose();

            VoiceChatManager.Instance.Gdpr.CanBeVoiceRecordedPlayerUserIds?.Remove(player.UserId);
            VoiceChatManager.Instance.Gdpr.Save();

            response = VoiceChatManager.Instance.Translation.YouWontBeVoiceRecordedWarning;
            return true;
        }
    }
}
