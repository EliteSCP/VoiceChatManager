// -----------------------------------------------------------------------
// <copyright file="IAudioConverter.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Audio.Capture
{
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using NAudio.Wave;
    using Xabe.FFmpeg;

    /// <summary>
    /// Converts audio files with a specific sample rate, channels and conversion format.
    /// </summary>
    public interface IAudioConverter
    {
        /// <summary>
        /// Gets the sample rate and number of channels.
        /// </summary>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// Gets the conversion format.
        /// </summary>
        Format FileFormat { get; }

        /// <summary>
        /// Gets the converted audio speed.
        /// </summary>
        float Speed { get; }

        /// <summary>
        /// Gets the converted audio bitrate.
        /// </summary>
        ushort Bitrate { get; }

        /// <summary>
        /// Gets a value indicating whether the file should be deleted or not after being converted.
        /// </summary>
        bool ShouldDeleteAfterConversion { get; }

        /// <summary>
        /// Gets the conversion preset, faster speed causes worse optimization and quality.
        /// </summary>
        ConversionPreset Preset { get; }

        /// <summary>
        /// Gets the maximum amount of conversions that can be done concurrently.
        /// </summary>
        int ConcurrentLimit { get; }

        /// <summary>
        /// Gets the conversion interval.
        /// </summary>
        int Interval { get; }

        /// <summary>
        /// Gets the queue of file paths to be converted.
        /// </summary>
        ConcurrentQueue<string> Queue { get; }

        /// <summary>
        /// Starts the conversion.
        /// </summary>
        /// <returns>Returns a <see cref="Task{TResult}"/>.</returns>
        Task StartAsync();

        /// <summary>
        /// Starts the conversion.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance.</param>
        /// <returns>Returns a <see cref="Task{TResult}"/>.</returns>
        Task StartAsync(CancellationToken cancellationToken);
    }
}
