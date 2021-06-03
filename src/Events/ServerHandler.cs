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
    using System.Threading;
    using System.Threading.Tasks;
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
            {
                FFmpeg.SetExecutablesPath(Instance.Config.FFmpegDirectoryPath);
            }
            else if (Instance.Config.Converter.IsEnabled)
            {
                Log.Warn($"Audio converter cannot be enabled, ffmpeg wasn't found at \"{Instance.Config.FFmpegDirectoryPath}\"");

                Instance.Config.Converter.IsEnabled = false;
            }

            Instance.Gdpr.Load();

            if (Instance.Config.Recorder.IsEnabled)
            {
                if (Instance.CaptureCancellationTokenSource == null)
                {
                    Instance.CaptureCancellationTokenSource = new CancellationTokenSource();
                    Instance.Capture = new VoiceChatCapture(
                        new WaveFormat(Instance.Config.Recorder.SampleRate, 1),
                        Instance.Config.Recorder.ReadBufferSize,
                        Instance.Config.Recorder.ReadInterval);

                    Task.Run(() => Instance.Capture.StartAsync(Instance.CaptureCancellationTokenSource.Token), Instance.CaptureCancellationTokenSource.Token);
                }

                foreach (var player in Player.List)
                {
                    if (!Instance.Gdpr.ShouldBeRespected ||
                        (Instance.Gdpr.ShouldBeRespected && (Instance.Gdpr.CanBeVoiceRecordedPlayerUserIds?.Contains(player.UserId) ?? false)))
                    {
                        player.SessionVariables["canBeVoiceRecorded"] = true;

                        var audioConverter = Instance.Config.Converter.IsEnabled ?
                            new AudioConverter(
                                new WaveFormat(Instance.Config.Converter.SampleRate, Instance.Config.Converter.Channels),
                                Instance.Config.Converter.FileFormat,
                                Instance.Config.Converter.Speed,
                                Instance.Config.Converter.Bitrate,
                                Instance.Config.Converter.ShouldDeleteAfterConversion,
                                Instance.Config.Converter.Preset)
                            : null;

                        var voiceChatRecorder = new VoiceChatRecorder(
                            player,
                            new WaveFormat(Instance.Config.Recorder.SampleRate, 1),
                            Path.Combine(Instance.Config.Recorder.RootDirectoryPath, RoundName),
                            Instance.Config.Recorder.DateTimeFormat,
                            Instance.Config.Recorder.MinimumBytesToWrite,
                            audioConverter);

                        if (!player.TryGet(out SamplePlaybackComponent playbackComponent)
                            || (!Instance.Capture?.Recorders.TryAdd(playbackComponent, voiceChatRecorder) ?? true))
                        {
                            Log.Debug($"Failed to add {player} ({player.UserId}) to the list of voice recorded players!", Instance.Config.IsDebugEnabled);
                            continue;
                        }

                        playbackComponent.MultiplyBySource = false;
                    }
                    else
                    {
                        IVoiceChatRecorder voiceChatRecorder = null;

                        if (!player.TryGet(out SamplePlaybackComponent playbackComponent)
                            || (!Instance.Capture?.Recorders.TryRemove(playbackComponent, out voiceChatRecorder) ?? true))
                        {
                            Log.Debug($"Failed to remove {player} ({player.UserId}) from the list of voice recorded players!", Instance.Config.IsDebugEnabled);
                            continue;
                        }

                        voiceChatRecorder?.Dispose();

                        player.SessionVariables.Remove("canBeVoiceRecorded");
                    }
                }
            }
            else
            {
                Instance.Capture?.Clear();

                Instance.CaptureCancellationTokenSource?.Cancel();
                Instance.CaptureCancellationTokenSource?.Dispose();
                Instance.CaptureCancellationTokenSource = null;

                Instance.Capture?.Dispose();
                Instance.Capture = null;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            var roundsCounter = PlayerStats.UptimeRounds - 1;

            if (Instance.Config.Recorder.IsEnabled
                && Instance.Config.Recorder.DeleteEveryNumberOfRounds > 0
                && roundsCounter > 0
                && roundsCounter % Instance.Config.Recorder.DeleteEveryNumberOfRounds == 0)
            {
                if (Directory.Exists(Instance.Config.Recorder.RootDirectoryPath))
                    Directory.Delete(Instance.Config.Recorder.RootDirectoryPath, true);
            }

            // It doesn't get invoked by Exiled
            if (Exiled.Events.Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
                OnReloadedConfigs();

            Server.Host.GameObject.AddComponent<VoiceReceiptTrigger>().RoomName = "SCP";

            RoundName = $"Round {DateTime.Now.ToString(Instance.Config.Recorder.DateTimeFormat)}";
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRestartingRound"/>
        public void OnRestartingRound() => Instance.Capture?.Clear();
    }
}
