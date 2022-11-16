// -----------------------------------------------------------------------
// <copyright file="Recorder.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Configs
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using Core.Audio.Capture;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;

    /// <summary>
    /// <see cref="IVoiceChatRecorder"/> related configs.
    /// </summary>
    public sealed class Recorder : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IVoiceChatRecorder"/> is enabled or not.
        /// </summary>
        [Description("Indicates whether the voice chat recorder is enabled or not.")]
        public bool IsEnabled { get; set; }

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

        /// <inheritdoc cref="IVoiceChatRecorder.TimeZone"/>
        [Description("The recorder timezone. Run \"TZUTIL.EXE /L\" in the command line of your machine to get all of the timezone ids")]
        public string TimeZone { get; private set; } = TimeZoneInfo.Local.Id;

        /// <inheritdoc cref="IVoiceChatRecorder.MinimumBytesToWrite"/>
        [Description("The minimum number of bytes required to write the audio, minimum is the read buffer size, 192000 bytes equals 1 second.")]
        public int MinimumBytesToWrite { get; private set; } = 192000;

        /// <summary>
        /// Gets the number of rounds which have to pass to delete the oldest round riectory. 1 means delete at every round restart, 0 means disabled.
        /// </summary>
        [Description("Keep audio files for a specific number of rounds, the oldest round directory will be deleted after surpassing this threshold. Set it to 1 to delete everything at every round restart and 0 to disable.")]
        public ushort KeepLastNumberOfRounds { get; private set; } = 0;
    }
}
