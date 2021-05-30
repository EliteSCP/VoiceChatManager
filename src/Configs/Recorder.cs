// -----------------------------------------------------------------------
// <copyright file="Recorder.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Configs
{
    using System.ComponentModel;
    using Api.Audio.Capture;

    /// <summary>
    /// <see cref="IVoiceChatRecorder"/> related configs.
    /// </summary>
    public sealed class Recorder
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="VoiceChatRecorder"/> is enabled or not.
        /// </summary>
        [Description("indicates whether the voice chat recorder is enabled or not")]
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Gets the <see cref="IVoiceChatRecorder"/> sample rate.
        /// </summary>
        [Description("The voice chat recorder sample rate")]
        public int SampleRate { get; private set; } = 48000;

        /// <summary>
        /// Gets the <see cref="IVoiceChatCapture.ReadBufferSize"/>, in bytes.
        /// </summary>
        [Description("The voice chat recorder read buffer size, in bytes, minimum is 960")]
        public int ReadBufferSize { get; private set; } = 1920;

        /// <summary>
        /// Gets the <see cref="IVoiceChatCapture.ReadInterval"/>, in milliseconds.
        /// </summary>
        [Description("The voice chat recorder read interval, in milliseconds, minimum is 20")]
        public int ReadInterval { get; private set; } = 40;
    }
}
