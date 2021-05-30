// -----------------------------------------------------------------------
// <copyright file="PlayerHandler.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Events
{
    using Api.Audio.Capture;
    using Api.Extensions;
    using Dissonance.Audio.Playback;
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
            if (!ev.Player.TryGet(out SamplePlaybackComponent samplePlaybackComponent))
                return;

            samplePlaybackComponent.MultiplyBySource = false;

            if (Instance.Config.Recorder.IsEnabled &&
                (!Instance.Gdpr.ShouldBeRespected ||
                (Instance.Gdpr.ShouldBeRespected && (Instance.Gdpr.CanBeVoiceRecordedPlayerUserIds?.Contains(ev.Player.UserId) ?? false))))
            {
                ev.Player.SessionVariables["canBeVoiceRecorded"] = true;

                if (!Instance.Capture.Recorders.TryAdd(samplePlaybackComponent, new VoiceChatRecorder(new WaveFormat(Instance.Config.Recorder.SampleRate, 1), ev.Player)))
                    Log.Debug($"Failed to add {ev.Player} ({ev.Player.UserId}) to the list of voice recorded players!", Instance.Config.IsDebugEnabled);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDestroying(DestroyingEventArgs)"/>
        public void OnDestroying(DestroyingEventArgs ev)
        {
            if (!ev.Player.TryGet(out SamplePlaybackComponent playbackComponent) || !Instance.Capture.Recorders.TryRemove(playbackComponent, out _))
                Log.Debug($"Failed to remove {ev.Player} ({ev.Player.UserId}) from the list of voice recorded players!", Instance.Config.IsDebugEnabled);
        }
    }
}
