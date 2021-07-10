// -----------------------------------------------------------------------
// <copyright file="PlayerProximityStreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Audio.Playback
{
    using System.IO;
    using Core.Audio.Capture;
    using Dissonance;
    using UnityEngine;
    using VoiceChatManager.Core.Logging;

    /// <inheritdoc cref="IPlayerProximityStreamedMicrophone"/>
    public class PlayerProximityStreamedMicrophone : ProximityStreamedMicrophone, IPlayerProximityStreamedMicrophone
    {
        /// <inheritdoc/>
        public virtual ITalker Talker { get; protected set; }

        /// <inheritdoc/>
        public override Vector3 Position => Talker?.Position ?? Vector3.zero;

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "PlayerProximityStreamedMicrophone";

        /// <summary>
        /// Inits the class.
        /// </summary>
        /// <param name="talker"><inheritdoc cref="Talker"/></param>
        /// <param name="stream"><inheritdoc cref="IStreamedMicrophone.Stream"/></param>
        /// <param name="volume"><inheritdoc cref="IStreamedMicrophone.Volume"/></param>
        /// <param name="channelName"><inheritdoc cref="IStreamedMicrophone.ChannelName"/></param>
        /// <param name="priority"><inheritdoc cref="IStreamedMicrophone.Priority"/></param>
        /// <param name="log"><inheritdoc cref="ILog"/></param>
        /// <returns>Returns the class instance.</returns>
        public virtual IPlayerProximityStreamedMicrophone Init(ITalker talker, Stream stream, float volume, string channelName, ChannelPriority priority = ChannelPriority.None, ILog log = null)
        {
            Talker = talker;

            Init(talker.Position, stream, volume, channelName, priority, log);

            return this;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool shouldDisposeAllResources)
        {
            Talker = null;

            base.Dispose(shouldDisposeAllResources);
        }
    }
}
