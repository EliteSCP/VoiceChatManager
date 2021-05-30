// -----------------------------------------------------------------------
// <copyright file="IPlayerProximityStreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Playback
{
    using Exiled.API.Features;

    /// <summary>
    /// A custom player proximity streamed microphone.
    /// </summary>
    public interface IPlayerProximityStreamedMicrophone : IProximityStreamedMicrophone
    {
        /// <summary>
        /// Gets the player which represents the center of the proximity streamed capture.
        /// </summary>
        Player Player { get; }
    }
}
