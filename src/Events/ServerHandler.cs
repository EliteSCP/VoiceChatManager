// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Events
{
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

                        if (!player.TryGet(out SamplePlaybackComponent playbackComponent) || !Instance.Capture.Recorders.TryAdd(playbackComponent, new VoiceChatRecorder(new WaveFormat(Instance.Config.Recorder.SampleRate, 1), player)))
                        {
                            Log.Debug($"Failed to add {player} ({player.UserId}) to the list of voice recorded players!", Instance.Config.IsDebugEnabled);
                            continue;
                        }

                        playbackComponent.MultiplyBySource = false;
                    }
                    else
                    {
                        if (!player.TryGet(out SamplePlaybackComponent playbackComponent) || !Instance.Capture.Recorders.TryRemove(playbackComponent, out _))
                        {
                            Log.Debug($"Failed to remove {player} ({player.UserId}) from the list of voice recorded players!", Instance.Config.IsDebugEnabled);
                            continue;
                        }

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
        }
    }
}
