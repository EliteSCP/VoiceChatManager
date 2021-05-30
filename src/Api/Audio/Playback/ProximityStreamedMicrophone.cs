// -----------------------------------------------------------------------
// <copyright file="ProximityStreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Playback
{
    using System.IO;
    using Dissonance;
    using UnityEngine;

    /// <inheritdoc cref="IProximityStreamedMicrophone"/>
    public class ProximityStreamedMicrophone : StreamedMicrophone, IProximityStreamedMicrophone
    {
        /// <inheritdoc/>
        public virtual Vector3 Position { get; protected set; }

        /// <inheritdoc/>
        public virtual ReferenceHub Dummy { get; protected set; }

        /// <inheritdoc/>
        public override bool IsThreeDimensional { get; } = true;

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

        /// <inheritdoc/>
        protected override void Dispose(bool shouldDisposeAllResources)
        {
            Destroy(Dummy);

            Dummy = null;

            base.Dispose(shouldDisposeAllResources);
        }
    }
}
