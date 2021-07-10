// -----------------------------------------------------------------------
// <copyright file="IPlayerProximityStreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Audio.Playback
{
    using VoiceChatManager.Core.Audio.Capture;

    /// <summary>
    /// A custom player proximity streamed microphone.
    /// </summary>
    public interface IPlayerProximityStreamedMicrophone : IProximityStreamedMicrophone
    {
        /// <summary>
        /// Gets the talker which represents the center of the proximity streamed capture.
        /// </summary>
        ITalker Talker { get; }
    }
}
