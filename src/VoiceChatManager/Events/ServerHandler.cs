// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Events
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Audio.Capture;
    using Core.Audio.Playback;
    using Core.Extensions;
    using Dissonance;
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

        /// <summary>
        /// Gets the round paths queue.
        /// </summary>
        internal ConcurrentQueue<string> RoundPaths { get; private set; } = new ConcurrentQueue<string>();

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnReloadedConfigs"/>
        public void OnReloadedConfigs()
        {
            Instance.Config.IsFFmpegInstalled = Directory.Exists(Instance.Config.FFmpegDirectoryPath);

            if (Instance.Config.IsFFmpegInstalled)
            {
                FFmpeg.SetExecutablesPath(Instance.Config.FFmpegDirectoryPath);
            }
            else if (Instance.Config.Converter.IsEnabled)
            {
                Log.Warn($"Audio converter cannot be enabled, FFmpeg wasn't found at \"{Instance.Config.FFmpegDirectoryPath}\"");

                Instance.Config.Converter.IsEnabled = false;
            }

            Instance.Gdpr.Load();

            if (Instance.Config.Converter.IsEnabled)
            {
                if (Instance.ConverterCancellationTokenSource == null)
                {
                    Instance.ConverterCancellationTokenSource = new CancellationTokenSource();
                    Instance.Converter = new AudioConverter(
                                new WaveFormat(Instance.Config.Converter.SampleRate, Instance.Config.Converter.Channels),
                                Instance.Config.Converter.FileFormat,
                                Instance.Config.Converter.Speed,
                                Instance.Config.Converter.Bitrate,
                                Instance.Config.Converter.ShouldDeleteAfterConversion,
                                Instance.Config.Converter.Preset,
                                Instance.Config.Converter.ConcurrentLimit,
                                Instance.Config.Converter.Interval);

                    Task.Run(() => Instance.Converter.StartAsync(Instance.ConverterCancellationTokenSource.Token)).ConfigureAwait(false);
                }
            }
            else
            {
                Instance.ConverterCancellationTokenSource?.Cancel();
                Instance.ConverterCancellationTokenSource?.Dispose();
                Instance.ConverterCancellationTokenSource = null;

                Instance.Converter?.Queue.Clear();
                Instance.Converter = null;
            }

            if (Instance.Config.Recorder.IsEnabled)
            {
                RoundPaths.Clear();

                if (Directory.Exists(Instance.Config.Recorder.RootDirectoryPath))
                {
                    foreach (var directory in new DirectoryInfo(Instance.Config.Recorder.RootDirectoryPath).GetDirectories().OrderBy(info => info.CreationTime))
                        RoundPaths.Enqueue(directory.FullName);
                }

                if (Instance.CaptureCancellationTokenSource == null)
                {
                    Instance.CaptureCancellationTokenSource = new CancellationTokenSource();
                    Instance.Capture = new VoiceChatCapture(
                        new WaveFormat(Instance.Config.Recorder.SampleRate, 1),
                        Instance.Config.Recorder.ReadBufferSize,
                        Instance.Config.Recorder.ReadInterval);

                    Task.Run(() => Instance.Capture.StartAsync(Instance.CaptureCancellationTokenSource.Token)).ConfigureAwait(false);
                }

                foreach (var player in Player.List)
                {
                    if (!Instance.Gdpr.IsCompliant ||
                        (Instance.Gdpr.IsCompliant && (Instance.Gdpr.CanBeVoiceRecordedPlayerUserIds?.Contains(player.UserId) ?? false)))
                    {
                        player.SessionVariables["canBeVoiceRecorded"] = true;

                        var talker = Talker.GetOrCreate(player.GameObject);
                        var voiceChatRecorder = new VoiceChatRecorder(
                            talker,
                            new WaveFormat(Instance.Config.Recorder.SampleRate, 1),
                            Path.Combine(Instance.Config.Recorder.RootDirectoryPath, RoundName),
                            Instance.Config.Recorder.DateTimeFormat,
                            Instance.Config.Recorder.MinimumBytesToWrite,
                            Instance.Converter);

                        if (talker.PlayBackComponent == null && (!Instance.Capture?.Recorders.TryAdd(talker, voiceChatRecorder) ?? true))
                        {
                            Log.Debug($"Failed to add {player} ({player.UserId}) to the list of voice recorded players!", Instance.Config.IsDebugEnabled);
                            continue;
                        }

                        talker.PlayBackComponent.MultiplyBySource = false;
                    }
                    else
                    {
                        IVoiceChatRecorder voiceChatRecorder = null;
                        ITalker talker = Talker.GetOrCreate(player.GameObject);

                        if (talker.PlayBackComponent == null || (!Instance.Capture?.Recorders.TryRemove(talker, out voiceChatRecorder) ?? true))
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
                RoundPaths.Clear();

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
            Server.Host.GameObject.AddComponent<VoiceReceiptTrigger>().RoomName = "SCP";

            // It doesn't get invoked by Exiled
            if (Exiled.Events.Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
                OnReloadedConfigs();

            RoundName = $"Round {DateTime.Now.ToString(Instance.Config.Recorder.DateTimeFormat)}";

            if (Instance.Config.Recorder.IsEnabled && Instance.Config.Recorder.KeepLastNumberOfRounds > 0)
            {
                Task.Run(() =>
                {
                    while (RoundPaths.Count >= Instance.Config.Recorder.KeepLastNumberOfRounds && RoundPaths.TryDequeue(out var path) && Directory.Exists(path))
                        Directory.Delete(path, true);

                    RoundPaths.Enqueue(Path.Combine(Instance.Config.Recorder.RootDirectoryPath, RoundName));
                }).ConfigureAwait(false);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRestartingRound"/>
        public void OnRestartingRound()
        {
            foreach (var streamedMicrophone in StreamedMicrophone.List)
                streamedMicrophone.Dispose();

            StreamedMicrophone.List.Clear();

            Instance.Capture?.Clear();
        }
    }
}
