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
    using Api.Audio.Capture;
    using Api.Extensions;
    using Api.Utilities;
    using Configs;
    using Events;
    using Exiled.API.Features;
    using HarmonyLib;

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
        public IVoiceChatCapture Capture { get; internal set; }

        /// <summary>
        /// Gets the <see cref="IAudioConverter"/> instance.
        /// </summary>
        public IAudioConverter Converter { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Capture"/> <see cref="CancellationTokenSource"/>.
        /// </summary>
        public CancellationTokenSource CaptureCancellationTokenSource { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Converter"/> <see cref="CancellationTokenSource"/>.
        /// </summary>
        public CancellationTokenSource ConverterCancellationTokenSource { get; internal set; }

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

            PlayerHandler = new PlayerHandler();
            ServerHandler = new ServerHandler();
            Gdpr = new Gdpr();

            // It doesn't get invoked by Exiled
            ServerHandler.OnReloadedConfigs();

            Exiled.Events.Handlers.Player.Verified += PlayerHandler.OnVerified;
            Exiled.Events.Handlers.Player.Destroying += PlayerHandler.OnDestroying;

            Exiled.Events.Handlers.Server.ReloadedConfigs += ServerHandler.OnReloadedConfigs;
            Exiled.Events.Handlers.Server.WaitingForPlayers += ServerHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RestartingRound += ServerHandler.OnRestartingRound;

            // Play on event events, OnWaitingForPlayers is invoked before this
            Exiled.Events.Handlers.Server.RoundStarted += ServerHandler.OnRoundStarted;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += ServerHandler.OnAnnouncingNtfEntrance;
            Exiled.Events.Handlers.Server.RespawningTeam += ServerHandler.OnRespawningTeam;
            Exiled.Events.Handlers.Warhead.Starting += ServerHandler.OnWarheadStarting;
            Exiled.Events.Handlers.Warhead.Stopping += ServerHandler.OnWarheadStopping;
            Exiled.Events.Handlers.Warhead.Detonated += ServerHandler.OnWarheadDetonated;
            Exiled.Events.Handlers.Map.Decontaminating += ServerHandler.OnDecontaminating;
            Exiled.Events.Handlers.Server.RoundEnded += ServerHandler.OnRoundEnded;

            harmonyInstance = new Harmony($"com.iopietro.voicechatmanager");
            harmonyInstance.PatchAll();

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            ServerHandler.RoundPaths.Clear();

            CaptureCancellationTokenSource?.Cancel();
            CaptureCancellationTokenSource?.Dispose();
            CaptureCancellationTokenSource = null;

            Capture?.Dispose();
            Capture = null;

            ConverterCancellationTokenSource?.Cancel();
            ConverterCancellationTokenSource?.Dispose();
            ConverterCancellationTokenSource = null;

            Converter?.Queue.Clear();
            Converter = null;

            Exiled.Events.Handlers.Player.Verified -= PlayerHandler.OnVerified;
            Exiled.Events.Handlers.Player.Destroying -= PlayerHandler.OnDestroying;

            Exiled.Events.Handlers.Server.ReloadedConfigs -= ServerHandler.OnReloadedConfigs;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= ServerHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RestartingRound -= ServerHandler.OnRestartingRound;

            // Play on event events, OnWaitingForPlayers is invoked before this
            Exiled.Events.Handlers.Server.RoundStarted -= ServerHandler.OnRoundStarted;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= ServerHandler.OnAnnouncingNtfEntrance;
            Exiled.Events.Handlers.Server.RespawningTeam -= ServerHandler.OnRespawningTeam;
            Exiled.Events.Handlers.Warhead.Starting -= ServerHandler.OnWarheadStarting;
            Exiled.Events.Handlers.Warhead.Stopping -= ServerHandler.OnWarheadStopping;
            Exiled.Events.Handlers.Warhead.Detonated -= ServerHandler.OnWarheadDetonated;
            Exiled.Events.Handlers.Map.Decontaminating -= ServerHandler.OnDecontaminating;
            Exiled.Events.Handlers.Server.RoundEnded -= ServerHandler.OnRoundEnded;

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
