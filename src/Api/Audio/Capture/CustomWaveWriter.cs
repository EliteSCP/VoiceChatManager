// -----------------------------------------------------------------------
// <copyright file="CustomWaveWriter.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Capture
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Extensions;
    using NAudio.Wave;

    /// <summary>
    /// Adds async support to <see cref="WaveFileWriter"/>.
    /// </summary>
    public class CustomWaveWriter : WaveFileWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomWaveWriter"/> class.
        /// </summary>
        /// <param name="outStream">The <see cref="Stream"/> to write in.</param>
        /// <param name="format">The audio <see cref="WaveFormat"/>.</param>
        public CustomWaveWriter(Stream outStream, WaveFormat format)
            : base(outStream, format)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomWaveWriter"/> class.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <param name="waveFormat">The audio <see cref="WaveFormat"/>.</param>
        public CustomWaveWriter(string filename, WaveFormat waveFormat)
            : base(filename.GetValidFilePath(".wav"), waveFormat)
        {
        }

        /// <summary>
        /// Writes samples in the <see cref="Stream"/>.
        /// </summary>
        /// <param name="samples">The samples to be written.</param>
        /// <returns>Returns a <see cref="ValueTask"/>.</returns>
        public async ValueTask WriteSamplesAsync(ArraySegment<byte> samples) => await WriteSamplesAsync(samples, default);

        /// <summary>
        /// Writes samples in the <see cref="Stream"/>.
        /// </summary>
        /// <param name="samples">The samples to be written.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance.</param>
        /// <returns>Returns a <see cref="ValueTask"/>.</returns>
        public async ValueTask WriteSamplesAsync(ArraySegment<byte> samples, CancellationToken cancellationToken)
        {
            await _outStream.WriteAsync(samples.Array, samples.Offset, samples.Count, cancellationToken);

            _dataChunkSize += samples.Count;
        }
    }
}
