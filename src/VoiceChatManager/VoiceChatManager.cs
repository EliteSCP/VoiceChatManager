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
    using Configs;
    using Core.Audio.Capture;
    using Core.Extensions;
    using Core.Utilities;
    using Events;
    using Exiled.API.Features;
    using HarmonyLib;
    using PlayerEvents = Exiled.Events.Handlers.Player;
    using ServerEvents = Exiled.Events.Handlers.Server;

    /// <summary>
    /// Plays audio files in game.
    /// </summary>
    public class VoiceChatManager : Plugin<Config, Translation>
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

            PlayerEvents.Verified += PlayerHandler.OnVerified;
            PlayerEvents.Destroying += PlayerHandler.OnDestroying;

            ServerEvents.ReloadedConfigs += ServerHandler.OnReloadedConfigs;
            ServerEvents.WaitingForPlayers += ServerHandler.OnWaitingForPlayers;
            ServerEvents.RestartingRound += ServerHandler.OnRestartingRound;

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

            PlayerEvents.Verified -= PlayerHandler.OnVerified;
            PlayerEvents.Destroying -= PlayerHandler.OnDestroying;

            ServerEvents.ReloadedConfigs -= ServerHandler.OnReloadedConfigs;
            ServerEvents.WaitingForPlayers -= ServerHandler.OnWaitingForPlayers;
            ServerEvents.RestartingRound -= ServerHandler.OnRestartingRound;

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
