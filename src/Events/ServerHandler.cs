// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Events
{
    using System;
    using System.IO;
    using Api.Audio.Capture;
    using Api.Extensions;
    using Dissonance;
    using Dissonance.Audio.Playback;
    using Exiled.API.Features;
    using NAudio.Wave;
    using Xabe.FFmpeg;
    using static VoiceChatManager;
    using Log = Exiled.API.Features.Log;

    /// <summary>
    /// Handles server-related events.
    /// </summary>
    internal sealed class ServerHandler
    {
        /// <summary>
        /// Gets the actual round name.
        /// </summary>
        public string RoundName { get; private set; }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnReloadedConfigs"/>
        public void OnReloadedConfigs()
        {
            if (!string.IsNullOrEmpty(Instance.Config.FFmpegDirectoryPath))
                FFmpeg.SetExecutablesPath(Instance.Config.FFmpegDirectoryPath);

            Instance.Gdpr.Load();

            if (Instance.Config.Recorder.IsEnabled)
            {
                foreach (var player in Player.List)
                {
                    if (!Instance.Gdpr.ShouldBeRespected ||
                        (Instance.Gdpr.ShouldBeRespected && (Instance.Gdpr.CanBeVoiceRecordedPlayerUserIds?.Contains(player.UserId) ?? false)))
                    {
                        player.SessionVariables["canBeVoiceRecorded"] = true;

                        var waveFormat = new WaveFormat(Instance.Config.Recorder.SampleRate, 1);
                        var voiceChatRecorder = new VoiceChatRecorder(
                            player,
                            waveFormat,
                            Path.Combine(Instance.Config.Recorder.RootDirectoryPath, RoundName),
                            Instance.Config.Recorder.DateTimeFormat,
                            Instance.Config.Recorder.MinimumBytesToWrite);

                        if (!player.TryGet(out SamplePlaybackComponent playbackComponent) || !Instance.Capture.Recorders.TryAdd(playbackComponent, voiceChatRecorder))
                        {
                            Log.Debug($"Failed to add {player} ({player.UserId}) to the list of voice recorded players!", Instance.Config.IsDebugEnabled);
                            continue;
                        }

                        playbackComponent.MultiplyBySource = false;
                    }
                    else
                    {
                        if (!player.TryGet(out SamplePlaybackComponent playbackComponent) || !Instance.Capture.Recorders.TryRemove(playbackComponent, out var voiceChatRecorder))
                        {
                            Log.Debug($"Failed to remove {player} ({player.UserId}) from the list of voice recorded players!", Instance.Config.IsDebugEnabled);
                            continue;
                        }

                        voiceChatRecorder.Dispose();

                        player.SessionVariables.Remove("canBeVoiceRecorded");
                    }
                }
            }
            else
            {
                Instance.Capture.Recorders.Clear();
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            if (Exiled.Events.Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
            {
                // It doesn't get invoked by Exiled
                OnReloadedConfigs();
            }

            Server.Host.GameObject.AddComponent<VoiceReceiptTrigger>().RoomName = "SCP";

            RoundName = $"Round {DateTime.Now.ToString(Instance.Config.Recorder.DateTimeFormat)}";
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRestartingRound"/>
        public void OnRestartingRound() => Instance.Capture.Clear();
    }
}
