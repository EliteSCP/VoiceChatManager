// -----------------------------------------------------------------------
// <copyright file="IProximityStreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Playback
{
    using UnityEngine;

    /// <summary>
    /// A custom proximity streamed microphone.
    /// </summary>
    public interface IProximityStreamedMicrophone : IStreamedMicrophone
    {
        /// <summary>
        /// Gets the stream proximity position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Gets the spawned dummy.
        /// </summary>
        ReferenceHub Dummy { get; }
    }
}
