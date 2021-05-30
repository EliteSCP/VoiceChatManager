// -----------------------------------------------------------------------
// <copyright file="PlayerProximityStreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Playback
{
    using System.IO;
    using Dissonance;
    using Exiled.API.Features;
    using UnityEngine;

    /// <inheritdoc cref="IPlayerProximityStreamedMicrophone"/>
    public class PlayerProximityStreamedMicrophone : ProximityStreamedMicrophone, IPlayerProximityStreamedMicrophone
    {
        /// <inheritdoc/>
        public virtual Player Player { get; protected set; }

        /// <inheritdoc/>
        public override Vector3 Position => Player.Position;

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "PlayerProximityStreamedMicrophone";

        /// <summary>
        /// Inits the class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="stream"><inheritdoc cref="IStreamedMicrophone.Stream"/></param>
        /// <param name="volume"><inheritdoc cref="IStreamedMicrophone.Volume"/></param>
        /// <param name="channelName"><inheritdoc cref="IStreamedMicrophone.ChannelName"/></param>
        /// <param name="priority"><inheritdoc cref="IStreamedMicrophone.Priority"/></param>
        /// <returns>Returns the class instance.</returns>
        public virtual IPlayerProximityStreamedMicrophone Init(Player player, Stream stream, float volume, string channelName, ChannelPriority priority = ChannelPriority.None)
        {
            Player = player;

            Init(player.Position, stream, volume, channelName, priority);

            return this;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool shouldDisposeAllResources)
        {
            Player = null;

            base.Dispose(shouldDisposeAllResources);
        }

        private void Update()
        {
            if (Dummy != null && Player.GameObject != null)
                Dummy.playerMovementSync.OverridePosition(Player.Position, 0f);
        }
    }
}
