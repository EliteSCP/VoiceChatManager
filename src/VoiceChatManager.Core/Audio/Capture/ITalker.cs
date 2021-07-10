// -----------------------------------------------------------------------
// <copyright file="ITalker.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Audio.Capture
{
    using Dissonance.Audio.Playback;
    using UnityEngine;

    /// <summary>
    /// Talker related properties.
    /// </summary>
    public interface ITalker
    {
        /// <summary>
        /// Gets the talker's nickname.
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// Gets the talker's user id.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Gets the talker's id.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the talker's role.
        /// </summary>
        RoleType Role { get; }

        /// <summary>
        /// Gets the talker's <see cref="ReferenceHub"/>.
        /// </summary>
        ReferenceHub ReferenceHub { get; }

        /// <summary>
        /// Gets the talker's <see cref="GameObject"/>.
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// Gets the talker's <see cref="IVoicePlayback"/>.
        /// </summary>
        IVoicePlayback VoicePlayback { get; }

        /// <summary>
        /// Gets the talker's <see cref="SamplePlaybackComponent"/>.
        /// </summary>
        SamplePlaybackComponent PlayBackComponent { get; }

        /// <summary>
        /// Gets the talker's position.
        /// </summary>
        Vector3 Position { get; }
    }
}
