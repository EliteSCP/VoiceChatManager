// -----------------------------------------------------------------------
// <copyright file="CaptureStatusType.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Enums
{
    /// <summary>
    /// Capture status types.
    /// </summary>
    public enum CaptureStatusType
    {
        /// <summary>
        /// The audio capture is stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// The audio capture is captured.
        /// </summary>
        Playing,

        /// <summary>
        /// The audio capture is paused.
        /// </summary>
        Paused,
    }
}
