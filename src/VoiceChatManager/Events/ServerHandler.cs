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
    using Core.Audio.Profiles;
    using Core.Extensions;
    using Dissonance;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
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
            Instance.Log = new Logging.Log(Instance.Config.IsDebugEnabled);

            Instance.Config.IsFFmpegInstalled = Directory.Exists(Instance.Config.FFmpegDirectoryPath);

            if (Instance.Config.IsFFmpegInstalled)
            {
                FFmpeg.SetExecutablesPath(Instance.Config.FFmpegDirectoryPath);
            }
            else if (Instance.Config.Converter.IsEnabled)
            {
                Log.Warn(string.Format(Instance.Translation.AudioConverterCannotBeEnabledError, Instance.Config.FFmpegDirectoryPath));

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
                            Instance.Config.Recorder.TimeZone,
                            Instance.Config.Recorder.MinimumBytesToWrite,
                            Instance.Converter);

                        if (talker.PlayBackComponent == null && (!Instance.Capture?.Recorders.TryAdd(talker, voiceChatRecorder) ?? true))
                        {
                            Log.Debug(string.Format(Instance.Translation.FailedToAddPlayerError, player.Nickname, player.UserId), Instance.Config.IsDebugEnabled);
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
                            Log.Debug(string.Format(Instance.Translation.FailedToRemovePlayerError, player.Nickname, player.UserId), Instance.Config.IsDebugEnabled);
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
            InitHost();

            // It doesn't get invoked by Exiled
            if (Exiled.Events.Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
                OnReloadedConfigs();

            RoundName = string.Format(
                Instance.Translation.RoundName,
                DateTime.Now.FromTimeZone(Instance.Config.Recorder.TimeZone).ToString(Instance.Config.Recorder.DateTimeFormat));

            if (Instance.Config.Recorder.IsEnabled && Instance.Config.Recorder.KeepLastNumberOfRounds > 0)
            {
                Task.Run(() =>
                {
                    while (RoundPaths.Count >= Instance.Config.Recorder.KeepLastNumberOfRounds && RoundPaths.TryDequeue(out var path) && Directory.Exists(path))
                        Directory.Delete(path, true);

                    RoundPaths.Enqueue(Path.Combine(Instance.Config.Recorder.RootDirectoryPath, RoundName));
                }).ConfigureAwait(false);
            }

            Instance.Config.PlayOnEvent.WaitingForPlayers.Play();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRestartingRound"/>
        public void OnRestartingRound()
        {
            foreach (var streamedMicrophone in StreamedMicrophone.List)
                streamedMicrophone.Dispose();

            StreamedMicrophone.List.Clear();

            Instance.Capture?.Clear();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted() => Instance.Config.PlayOnEvent.RoundStarted.Play();

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs)"/>
        public void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev) => ev.IsAllowed = !Instance.Config.PlayOnEvent.NtfSpawned.Play();

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRespawningTeam(RespawningTeamEventArgs)"/>
        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
                Instance.Config.PlayOnEvent.ChaosInsurgencySpawned.Play();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Warhead.OnStarting(StartingEventArgs)"/>
        public void OnWarheadStarting(StartingEventArgs ev) => Instance.Config.PlayOnEvent.WarheadStart.Play();

        /// <inheritdoc cref="Exiled.Events.Handlers.Warhead.OnStopping(StoppingEventArgs)"/>
        public void OnWarheadStopping(StoppingEventArgs ev) => Instance.Config.PlayOnEvent.WarheadCanceled.Play();

        /// <inheritdoc cref="Exiled.Events.Handlers.Warhead.OnDetonated"/>
        public void OnWarheadDetonated() => Instance.Config.PlayOnEvent.WarheadDetonated.Play();

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnDecontaminating(DecontaminatingEventArgs)"/>
        public void OnDecontaminating(DecontaminatingEventArgs ev) => Instance.Config.PlayOnEvent.DecontaminationStarted.Play();

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundEnded(RoundEndedEventArgs)"/>
        public void OnRoundEnded(RoundEndedEventArgs ev) => Instance.Config.PlayOnEvent.RoundEnded.Play();

        private void InitHost()
        {
            Server.Host.Radio.Network_syncPrimaryVoicechatButton = true;
            Server.Host.ReferenceHub.characterClassManager.CurClass = RoleType.ClassD;
            Server.Host.DisplayNickname = Instance.Config.DedicatedServerName;
            Server.Host.GameObject.AddComponent<VoiceReceiptTrigger>().RoomName = "SCP";

            Server.Host.DissonanceUserSetup.SetProfile(new ServerHostProfile(Server.Host.DissonanceUserSetup));
        }
    }
}
