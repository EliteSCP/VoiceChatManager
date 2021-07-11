// -----------------------------------------------------------------------
// <copyright file="PlayerHandler.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Events
{
    using System.IO;
    using Core.Audio.Capture;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using NAudio.Wave;
    using static VoiceChatManager;

    /// <summary>
    /// Handles player-related events.
    /// </summary>
    internal sealed class PlayerHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnVerified(VerifiedEventArgs)"/>
        public void OnVerified(VerifiedEventArgs ev)
        {
            var talker = Talker.GetOrCreate(ev.Player.GameObject);

            if (talker.PlayBackComponent == null)
                return;

            talker.PlayBackComponent.MultiplyBySource = false;

            if (Instance.Config.Recorder.IsEnabled &&
                (!Instance.Gdpr.IsCompliant ||
                (Instance.Gdpr.IsCompliant && (Instance.Gdpr.CanBeVoiceRecordedPlayerUserIds?.Contains(ev.Player.UserId) ?? false))))
            {
                ev.Player.SessionVariables["canBeVoiceRecorded"] = true;

                var voiceChatRecorder = new VoiceChatRecorder(
                    Talker.GetOrCreate(ev.Player.GameObject),
                    new WaveFormat(Instance.Config.Recorder.SampleRate, 1),
                    Path.Combine(Instance.Config.Recorder.RootDirectoryPath, Instance.ServerHandler.RoundName),
                    Instance.Config.Recorder.DateTimeFormat,
                    Instance.Config.Recorder.MinimumBytesToWrite,
                    Instance.Converter);

                if (!Instance.Capture?.Recorders.TryAdd(voiceChatRecorder.Talker, voiceChatRecorder) ?? true)
                    Log.Debug(string.Format(Instance.Translation.FailedToAddPlayerError, ev.Player.Nickname, ev.Player.UserId), Instance.Config.IsDebugEnabled);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDestroying(DestroyingEventArgs)"/>
        public void OnDestroying(DestroyingEventArgs ev)
        {
            IVoiceChatRecorder voiceChatRecorder = null;
            ITalker talker = Talker.GetOrCreate(ev.Player.GameObject);

            if (talker.PlayBackComponent == null || (!Instance.Capture?.Recorders.TryRemove(talker, out voiceChatRecorder) ?? true))
            {
                Log.Debug(string.Format(Instance.Translation.FailedToRemovePlayerError, ev.Player.Nickname, ev.Player.UserId), Instance.Config.IsDebugEnabled);
                return;
            }

            voiceChatRecorder?.Dispose();
        }
    }
}
