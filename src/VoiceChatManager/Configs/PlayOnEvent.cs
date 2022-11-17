// -----------------------------------------------------------------------
// <copyright file="PlayOnEvent.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Configs
{
    using System.ComponentModel;

    /// <summary>
    /// <see cref="PlayOnEvent"/> related configs.
    /// </summary>
    public sealed class PlayOnEvent
    {
        /// <summary>
        /// Gets <see cref="AudioFile"/> played on WaitingForPlayers event.
        /// </summary>
        [Description("Called when the server waits for players:")]
        public AudioFile WaitingForPlayers { get; private set; } = new AudioFile();

        /// <summary>
        /// Gets <see cref="AudioFile"/> played on RoundStarted event.
        /// </summary>
        [Description("Called when the round has started:")]
        public AudioFile RoundStarted { get; private set; } = new AudioFile();

        /// <summary>
        /// Gets <see cref="AudioFile"/> played on NtfRespawn event.
        /// </summary>
        [Description("Called when the NTF are spawned:")]
        public AudioFile NtfSpawned { get; private set; } = new AudioFile();

        /// <summary>
        /// Gets <see cref="AudioFile"/> played on ChaosRespawn event.
        /// </summary>
        [Description("Called when the CI are spawned: (will be heard by ALL players not only ClassD and Chaos!")]
        public AudioFile ChaosInsurgencySpawned { get; private set; } = new AudioFile();

        /// <summary>
        /// Gets <see cref="AudioFile"/> played on WarheadStart event.
        /// </summary>
        [Description("Called when the Warhead is started: (the default Alpha Warhead cassie WILL still play!)")]
        public AudioFile WarheadStart { get; private set; } = new AudioFile();

        /// <summary>
        /// Gets <see cref="AudioFile"/> played on WarheadCancel event.
        /// </summary>
        [Description("Called when the Warhead is cancaled: (the default Alpha Warhead cassie WILL still play!)")]
        public AudioFile WarheadCanceled { get; private set; } = new AudioFile();

        /// <summary>
        /// Gets <see cref="AudioFile"/> played on WarheadDetonated event.
        /// </summary>
        [Description("Called when the Warhead is detonated:")]
        public AudioFile WarheadDetonated { get; private set; } = new AudioFile();

        /// <summary>
        /// Gets <see cref="AudioFile"/> played on DecontaminationStart event.
        /// </summary>
        [Description("Called when the Decontamination proccess has started: (the default decontamination cassie WILL still play!)")]
        public AudioFile DecontaminationStarted { get; private set; } = new AudioFile();

        /// <summary>
        /// Gets <see cref="AudioFile"/> played on RoundEnded event.
        /// </summary>
        [Description("Called when the round has ended:")]
        public AudioFile RoundEnded { get; private set; } = new AudioFile();
    }
}
