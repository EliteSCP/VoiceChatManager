// -----------------------------------------------------------------------
// <copyright file="MicrophoneStatusType.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Enums
{
    /// <summary>
    /// Microphone status types.
    /// </summary>
    public enum MicrophoneStatusType
    {
        /// <summary>
        /// The microphone capture is stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// The microphone is being captured.
        /// </summary>
        Playing,

        /// <summary>
        /// The microphone capture is stopped.
        /// </summary>
        Paused,
    }
}
