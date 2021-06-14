// -----------------------------------------------------------------------
// <copyright file="ProximityStreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Playback
{
    using System.IO;
    using Api.Enums;
    using Dissonance;
    using Exiled.API.Features;
    using UnityEngine;

    /// <inheritdoc cref="IProximityStreamedMicrophone"/>
    public class ProximityStreamedMicrophone : StreamedMicrophone, IProximityStreamedMicrophone
    {
        private float lastSyncedTime;

        /// <inheritdoc/>
        public virtual Vector3 Position { get; protected set; }

        /// <inheritdoc/>
        public virtual ReferenceHub Dummy => Server.Host.ReferenceHub;

        /// <inheritdoc/>
        public override bool IsThreeDimensional { get; set; } = true;

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "ProximityStreamedMicrophone";

        /// <summary>
        /// Inits the class.
        /// </summary>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="stream"><inheritdoc cref="StreamedMicrophone.Stream"/></param>
        /// <param name="volume"><inheritdoc cref="StreamedMicrophone.Volume"/></param>
        /// <param name="channelName"><inheritdoc cref="StreamedMicrophone.ChannelName"/></param>
        /// <param name="priority"><inheritdoc cref="StreamedMicrophone.Priority"/></param>
        /// <returns>Returns the class instance.</returns>
        public virtual IProximityStreamedMicrophone Init(Vector3 position, Stream stream, float volume, string channelName, ChannelPriority priority = ChannelPriority.None)
        {
            Position = position;

            Init(stream, volume, channelName, priority);

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
            foreach (var player in Player.List)
            {
                if ((Position - player.Position).sqrMagnitude > 375)
                    continue;

                Server.SendSpawnMessage?.Invoke(null, new object[] { Dummy.networkIdentity, player.Connection });
            }
        }
    }
}
