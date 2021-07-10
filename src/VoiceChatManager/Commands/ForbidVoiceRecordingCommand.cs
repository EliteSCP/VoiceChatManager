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
        public string Description { get; } = "Type this command to forbid to be voice recorded for security reasons.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VoiceChatManager.Instance.Gdpr.IsCompliant || !VoiceChatManager.Instance.Config.Recorder.IsEnabled)
            {
                response = "Command is currently disabled.";
                return false;
            }

            if (!(Player.Get(sender as CommandSender) is Player player))
            {
                response = "An error has occurred while executing the command!";
                return false;
            }

            if (!player.SessionVariables.TryGetValue("canBeVoiceRecorded", out object canBeVoiceRecorded) || !(bool)canBeVoiceRecorded)
            {
                response = "YOU'RE NOT BEING VOICE RECORDED YET!\nTYPE .acceptvoicerecording TO ACCEPT TO BE VOICE RECORDED FOR SECURITY REASONS.";
                return false;
            }

            player.SessionVariables.Remove("canBeVoiceRecorded");

            IVoiceChatRecorder voiceChatRecorder = null;
            ITalker talker = Talker.GetOrCreate(player.GameObject);

            if (talker.PlayBackComponent == null || (!VoiceChatManager.Instance.Capture?.Recorders.TryRemove(talker, out voiceChatRecorder) ?? true))
            {
                response = "An error has occurred! You cannot be removed from the list of voice recorded players!";
                return true;
            }

            voiceChatRecorder?.Dispose();

            VoiceChatManager.Instance.Gdpr.CanBeVoiceRecordedPlayerUserIds?.Remove(player.UserId);
            VoiceChatManager.Instance.Gdpr.Save();

            response = "FROM NOW ON, YOU WON'T BE VOICE RECORDED ANYMORE.";
            return true;
        }
    }
}
