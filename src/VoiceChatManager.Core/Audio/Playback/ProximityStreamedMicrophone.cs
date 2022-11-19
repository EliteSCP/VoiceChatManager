// -----------------------------------------------------------------------
// <copyright file="ProximityStreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Audio.Playback
{
    using System.IO;
    using Assets._Scripts.Dissonance;
    using Core.Enums;
    using Core.Logging;
    using Core.Utilities;
    using Dissonance;
    using UnityEngine;

    /// <inheritdoc cref="IProximityStreamedMicrophone"/>
    public class ProximityStreamedMicrophone : StreamedMicrophone, IProximityStreamedMicrophone
    {
        private float lastSyncedTime;

        /// <inheritdoc/>
        public virtual Vector3 Position { get; protected set; }

        /// <inheritdoc/>
        public virtual ReferenceHub Dummy => ReferenceHub.HostHub;

        /// <inheritdoc/>
        public override SpeakingFlags SpeakingFlags => 0;

        /// <inheritdoc/>
        public override bool IsThreeDimensional { get; set; } = true;

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "ProximityStreamedMicrophone";

        /// <summary>
        /// Inits the class.
        /// </summary>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="stream"><inheritdoc cref="IStreamedMicrophone.Stream"/></param>
        /// <param name="volume"><inheritdoc cref="IStreamedMicrophone.Volume"/></param>
        /// <param name="channelName"><inheritdoc cref="IStreamedMicrophone.ChannelName"/></param>
        /// <param name="priority"><inheritdoc cref="IStreamedMicrophone.Priority"/></param>
        /// <param name="log"><inheritdoc cref="ILog"/></param>
        /// <returns>Returns the class instance.</returns>
        public virtual IProximityStreamedMicrophone Init(Vector3 position, Stream stream, float volume, string channelName, ChannelPriority priority = ChannelPriority.None, ILog log = null)
        {
            Position = position;

            Init(stream, volume, channelName, priority, log);

            return this;
        }

        /// <summary>
        /// Syncs the host position to every player near it.
        /// </summary>
        private void Update()
        {
            if (Dummy == null || Status != CaptureStatusType.Playing)
                return;

            if (lastSyncedTime < 0.1)
            {
                lastSyncedTime += Time.unscaledDeltaTime;
                return;
            }

            lastSyncedTime = 0;

            Dummy.transform.localScale = Vector3.zero;
            Dummy.transform.localPosition = Position;

            // Updates the host position to every player near it and bypasses client-side local player checks.
            foreach (var player in ReferenceHub.Hubs.Values)
            {
                if (player == ReferenceHub.HostHub || (Position - player.playerMovementSync.GetRealPosition()).sqrMagnitude > 375)
                    continue;

                CachedProperties.SendSpawnMessage(Dummy.networkIdentity, player.scp079PlayerScript.connectionToClient);
            }
        }
    }
}
