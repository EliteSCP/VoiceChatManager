// -----------------------------------------------------------------------
// <copyright file="AudioConverter.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Capture
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
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
        /// <param name="fileFormat"><inheritdoc cref="FileFormat"/></param>
        /// <param name="speed"><inheritdoc cref="Speed"/></param>
        /// <param name="bitrate"><inheritdoc cref="Bitrate"/></param>
        /// <param name="shouldDeleteAfterConversion"><inheritdoc cref="ShouldDeleteAfterConversion"/></param>
        /// <param name="preset"><inheritdoc cref="Preset"/></param>
        public AudioConverter(WaveFormat waveFormat, Format fileFormat, float speed, ushort bitrate, bool shouldDeleteAfterConversion, ConversionPreset preset)
            : this(waveFormat, fileFormat, speed, bitrate, shouldDeleteAfterConversion, preset, 2)
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
        /// <param name="preset"><inheritdoc cref="Preset"/></param>
        /// <param name="concurrentConversions"><inheritdoc cref="ConcurrentLimit"/></param>
        public AudioConverter(WaveFormat waveFormat, Format fileFormat, float speed, ushort bitrate, bool shouldDeleteAfterConversion, ConversionPreset preset, int concurrentConversions)
            : this(waveFormat, fileFormat, speed, bitrate, shouldDeleteAfterConversion, preset, concurrentConversions, 1000)
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
        /// <param name="concurrentConversions"><inheritdoc cref="ConcurrentLimit"/></param>
        /// <param name="interval"><inheritdoc cref="Interval"/></param>
        public AudioConverter(
            WaveFormat waveFormat,
            Format format,
            float speed,
            ushort bitrate,
            bool shouldDeleteAfterConversion,
            ConversionPreset preset,
            int concurrentConversions,
            int interval)
        {
            WaveFormat = waveFormat;
            FileFormat = format;
            Speed = speed;
            Bitrate = bitrate;
            ShouldDeleteAfterConversion = shouldDeleteAfterConversion;
            Preset = preset;
            ConcurrentLimit = concurrentConversions;
            Interval = interval;
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
        public int ConcurrentLimit { get; }

        /// <inheritdoc/>
        public int Interval { get; }

        /// <inheritdoc/>
        public ConcurrentQueue<string> Queue { get; } = new ConcurrentQueue<string>();

        /// <inheritdoc/>
        public async Task StartAsync() => await StartAsync(default).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var filesToConvert = new List<string>(32);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(Interval, cancellationToken).ConfigureAwait(false);

                if (Queue.IsEmpty)
                    continue;

                for (int i = ConcurrentLimit; i > 0; i--)
                {
                    if (Queue.TryDequeue(out var path))
                        filesToConvert.Add(path);

                    if (Queue.IsEmpty)
                        break;
                }

                await filesToConvert.ParallelForEachAsync(
                    async path =>
                    {
                        try
                        {
                            await path.ConvertFileAsync(WaveFormat.SampleRate, WaveFormat.Channels, Speed, FileFormat, Preset, extraParameters: $"-ab {Bitrate}k").ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            Log.Error($"{typeof(AudioConverter).FullName}{nameof(StartAsync)} Failed to convert \"{path}\", error:\n{exception}");
                        }

                        if (ShouldDeleteAfterConversion && File.Exists(path))
                            File.Delete(path);
                    },
                    cancellationToken,
                    ConcurrentLimit).ConfigureAwait(false);

                filesToConvert.Clear();
            }
        }
    }
}
