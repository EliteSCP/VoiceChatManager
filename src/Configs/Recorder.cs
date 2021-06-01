// -----------------------------------------------------------------------
// <copyright file="Recorder.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Configs
{
    using System.ComponentModel;
    using System.IO;
    using Api.Audio.Capture;
    using Exiled.API.Features;

    /// <summary>
    /// <see cref="IVoiceChatRecorder"/> related configs.
    /// </summary>
    public sealed class Recorder
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="VoiceChatRecorder"/> is enabled or not.
        /// </summary>
        [Description("indicates whether the voice chat recorder is enabled or not.")]
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Gets the <see cref="IVoiceChatRecorder"/> sample rate.
        /// </summary>
        [Description("The voice chat recorder sample rate.")]
        public int SampleRate { get; private set; } = 48000;

        /// <inheritdoc cref="IVoiceChatCapture.ReadBufferSize"/>
        [Description("The voice chat recorder read buffer size, in bytes, minimum is 960.")]
        public int ReadBufferSize { get; private set; } = 1920;

        /// <inheritdoc cref="IVoiceChatCapture.ReadInterval"/>
        [Description("The voice chat recorder read interval, in milliseconds, minimum is 20.")]
        public int ReadInterval { get; private set; } = 40;

        /// <inheritdoc cref="IVoiceChatRecorder.RootDirectoryPath"/>
        [Description("The root directory path, at which audio files will be saved.")]
        public string RootDirectoryPath { get; private set; } = Path.Combine(Paths.Plugins, "VoiceChatManager", "Recordings");

        /// <inheritdoc cref="IVoiceChatRecorder.DateTimeFormat"/>
        [Description("The date time format that will be written in the file name")]
        public string DateTimeFormat { get; private set; } = "dd-MM-yy HH.mm.ss.fff";

        /// <inheritdoc cref="IVoiceChatRecorder.MinimumBytesToWrite"/>
        [Description("The minimum number of bytes required to write the audio, minimum is the read buffer size.")]
        public int MinimumBytesToWrite { get; private set; } = 48000;
    }
}
