// -----------------------------------------------------------------------
// <copyright file="Converter.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Configs
{
    using System;
    using System.ComponentModel;
    using Api.Audio.Capture;
    using Exiled.API.Interfaces;
    using Xabe.FFmpeg;

    /// <summary>
    /// Audio converter related configs.
    /// </summary>
    public sealed class Converter : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IVoiceChatRecorder"/> is enabled or not.
        /// </summary>
        [Description("Indicates whether the voice chat audio converter is enabled or not.")]
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc cref="IAudioConverter.ShouldDeleteAfterConversion"/>
        [Description("Indicates whether the file should be deleted or not after being converted.")]
        public bool ShouldDeleteAfterConversion { get; private set; } = true;

        /// <summary>
        /// Gets the converted audio sample rate.
        /// </summary>
        [Description("The converted audio sample rate.")]
        public int SampleRate { get; private set; } = 48000;

        /// <summary>
        /// Gets the converted audio number of channels.
        /// </summary>
        [Description("The converted audio number of channels.")]
        public int Channels { get; private set; } = 1;

        /// <inheritdoc cref="IAudioConverter.FileFormat"/>
        [Description("The conversion format. Recommended: adts, mp3")]
        public Format FileFormat { get; private set; } = Format.adts;

        /// <inheritdoc cref="IAudioConverter.Speed"/>
        [Description("The converted audio speed.")]
        public float Speed { get; private set; } = 1;

        /// <inheritdoc cref="IAudioConverter.Preset"/>
        [Description("The convertion preset. Available: VerySlow, Slower, Slow, Medium, Fast, Faster, VeryFast, SuperFast, UltraFast")]
        public ConversionPreset Preset { get; private set; } = ConversionPreset.UltraFast;

        /// <inheritdoc cref="IAudioConverter.Bitrate"/>
        [Description("The converted audio bitrate, in kbps. The lower the bitrate, the less space will be occupied.")]
        public ushort Bitrate { get; private set; } = 128;

        /// <inheritdoc cref="IAudioConverter.ConcurrentLimit"/>
        [Description("The maximum amount of conversions that can be done concurrently.")]
        public int ConcurrentLimit { get; private set; } = Environment.ProcessorCount - 1;

        /// <inheritdoc cref="IAudioConverter.Interval"/>
        [Description("The conversion interval, in milliseconds.")]
        public int Interval { get; private set; } = 0;
    }
}
