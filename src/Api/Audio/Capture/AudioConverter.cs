// -----------------------------------------------------------------------
// <copyright file="AudioConverter.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Capture
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Api.Extensions;
    using Exiled.API.Features;
    using NAudio.Wave;
    using Xabe.FFmpeg;

    /// <inheritdoc/>
    public class AudioConverter : IAudioConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioConverter"/> class.
        /// </summary>
        public AudioConverter()
            : this(new WaveFormat(48000, 1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioConverter"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        public AudioConverter(WaveFormat waveFormat)
            : this(waveFormat, Format.mp3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioConverter"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="fileFormat"><inheritdoc cref="FileFormat"/></param>
        public AudioConverter(WaveFormat waveFormat, Format fileFormat)
            : this(waveFormat, fileFormat, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioConverter"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="fileFormat"><inheritdoc cref="FileFormat"/></param>
        /// <param name="speed"><inheritdoc cref="Speed"/></param>
        public AudioConverter(WaveFormat waveFormat, Format fileFormat, float speed)
            : this(waveFormat, fileFormat, speed, 192)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioConverter"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="fileFormat"><inheritdoc cref="FileFormat"/></param>
        /// <param name="speed"><inheritdoc cref="Speed"/></param>
        /// <param name="bitrate"><inheritdoc cref="Bitrate"/></param>
        public AudioConverter(WaveFormat waveFormat, Format fileFormat, float speed, ushort bitrate)
            : this(waveFormat, fileFormat, speed, bitrate, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioConverter"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="fileFormat"><inheritdoc cref="FileFormat"/></param>
        /// <param name="speed"><inheritdoc cref="Speed"/></param>
        /// <param name="bitrate"><inheritdoc cref="Bitrate"/></param>
        /// <param name="shouldDeleteAfterConversion"><inheritdoc cref="ShouldDeleteAfterConversion"/></param>
        public AudioConverter(WaveFormat waveFormat, Format fileFormat, float speed, ushort bitrate, bool shouldDeleteAfterConversion)
            : this(waveFormat, fileFormat, speed, bitrate, shouldDeleteAfterConversion, ConversionPreset.Medium)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioConverter"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="format"><inheritdoc cref="FileFormat"/></param>
        /// <param name="speed"><inheritdoc cref="Speed"/></param>
        /// <param name="bitrate"><inheritdoc cref="Bitrate"/></param>
        /// <param name="shouldDeleteAfterConversion"><inheritdoc cref="ShouldDeleteAfterConversion"/></param>
        /// <param name="preset"><inheritdoc cref="Preset"/></param>
        public AudioConverter(WaveFormat waveFormat, Format format, float speed, ushort bitrate, bool shouldDeleteAfterConversion, ConversionPreset preset)
        {
            WaveFormat = waveFormat;
            FileFormat = format;
            Speed = speed;
            Bitrate = bitrate;
            ShouldDeleteAfterConversion = shouldDeleteAfterConversion;
            Preset = preset;
        }

        /// <inheritdoc/>
        public bool ShouldDeleteAfterConversion { get; }

        /// <inheritdoc/>
        public WaveFormat WaveFormat { get; }

        /// <inheritdoc/>
        public Format FileFormat { get; }

        /// <inheritdoc/>
        public float Speed { get; }

        /// <inheritdoc/>
        public ushort Bitrate { get; }

        /// <inheritdoc/>
        public ConversionPreset Preset { get; }

        /// <inheritdoc/>
        public async Task StartAsync(string path)
        {
            try
            {
                await path.ConvertFileAsync(WaveFormat.SampleRate, WaveFormat.Channels, Speed, FileFormat, Preset, extraParameters: $"-ab {Bitrate}k");

                if (ShouldDeleteAfterConversion && File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(AudioConverter).FullName}{nameof(StartAsync)} Failed to convert \"{path}\", error:\n{exception}");
            }
        }
    }
}
