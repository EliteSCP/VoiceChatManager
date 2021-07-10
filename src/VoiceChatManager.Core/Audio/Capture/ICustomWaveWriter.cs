// -----------------------------------------------------------------------
// <copyright file="ICustomWaveWriter.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Audio.Capture
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using NAudio.Wave;

    /// <summary>
    /// Adds async support to a <see cref="WaveFileWriter"/>.
    /// </summary>
    public interface ICustomWaveWriter : IDisposable
    {
        /// <summary>
        /// Gets the file length.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// Writes samples in the <see cref="Stream"/>.
        /// </summary>
        /// <param name="samples">The samples to be written.</param>
        /// <returns>Returns a <see cref="ValueTask"/>.</returns>
        ValueTask WriteSamplesAsync(ArraySegment<byte> samples);

        /// <summary>
        /// Writes samples in the <see cref="Stream"/>.
        /// </summary>
        /// <param name="samples">The samples to be written.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance.</param>
        /// <returns>Returns a <see cref="ValueTask"/>.</returns>
        ValueTask WriteSamplesAsync(ArraySegment<byte> samples, CancellationToken cancellationToken);
    }
}
