// -----------------------------------------------------------------------
// <copyright file="VoiceChatManager.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Audio.Capture;
    using Api.Utilities;
    using Configs;
    using Events;
    using Exiled.API.Features;
    using HarmonyLib;
    using NAudio.Wave;

    /// <summary>
    /// Plays audio files in game.
    /// </summary>
    public class VoiceChatManager : Plugin<Config>
    {
        private static readonly VoiceChatManager InstanceValue = new VoiceChatManager();
        private static Harmony harmonyInstance;

        private VoiceChatManager()
        {
        }

        /// <summary>
        /// Gets the plugin singleton instance.
        /// </summary>
        public static VoiceChatManager Instance => InstanceValue;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(2, 8, 0);

        /// <summary>
        /// Gets GDPR related configs.
        /// </summary>
        public Gdpr Gdpr { get; private set; }

        /// <summary>
        /// Gets the <see cref="IVoiceChatCapture"/>.
        /// </summary>
        public IVoiceChatCapture Capture { get; private set; }

        /// <summary>
        /// Gets the <see cref="Capture"/> <see cref="CancellationTokenSource"/>.
        /// </summary>
        public CancellationTokenSource CaptureCancellationTokenSource { get; private set; }

        /// <summary>
        /// Gets the <see cref="PlayerHandler"/> instance.
        /// </summary>
        internal PlayerHandler PlayerHandler { get; private set; }

        /// <summary>
        /// Gets the <see cref="ServerHandler"/> instance.
        /// </summary>
        internal ServerHandler ServerHandler { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            CachedProperties.MaximumVoiceChatDesync = TimeSpan.FromSeconds(60);

            CaptureCancellationTokenSource = new CancellationTokenSource();

            PlayerHandler = new PlayerHandler();
            ServerHandler = new ServerHandler();
            Gdpr = new Gdpr();
            Capture = new VoiceChatCapture(new WaveFormat(Instance.Config.Recorder.SampleRate, 1), Config.Recorder.ReadBufferSize, Config.Recorder.ReadInterval);

            if (Config.Recorder.IsEnabled)
                Task.Run(() => Capture.StartAsync(CaptureCancellationTokenSource.Token), CaptureCancellationTokenSource.Token);

            // It doesn't get invoked by Exiled
            ServerHandler.OnReloadedConfigs();

            Exiled.Events.Handlers.Player.Verified += PlayerHandler.OnVerified;
            Exiled.Events.Handlers.Player.Destroying += PlayerHandler.OnDestroying;

            Exiled.Events.Handlers.Server.ReloadedConfigs += ServerHandler.OnReloadedConfigs;
            Exiled.Events.Handlers.Server.WaitingForPlayers += ServerHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RestartingRound += ServerHandler.OnRestartingRound;

            harmonyInstance = new Harmony($"voicechatmanager.scpsl.{DateTime.Now.Ticks}");
            harmonyInstance.PatchAll();

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            CaptureCancellationTokenSource.Cancel();
            CaptureCancellationTokenSource.Dispose();

            Capture.Dispose();

            Exiled.Events.Handlers.Player.Verified -= PlayerHandler.OnVerified;
            Exiled.Events.Handlers.Player.Destroying -= PlayerHandler.OnDestroying;

            Exiled.Events.Handlers.Server.ReloadedConfigs -= ServerHandler.OnReloadedConfigs;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= ServerHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RestartingRound -= ServerHandler.OnRestartingRound;

            harmonyInstance.UnpatchAll();
            harmonyInstance = null;
            PlayerHandler = null;
            ServerHandler = null;
            Gdpr = null;

            foreach (var player in Player.List)
                player.SessionVariables.Remove("canBeVoiceRecorded");

            base.OnDisabled();
        }
    }
}
