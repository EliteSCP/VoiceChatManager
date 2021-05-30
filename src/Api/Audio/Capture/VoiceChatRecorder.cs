// -----------------------------------------------------------------------
// <copyright file="VoiceChatRecorder.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Capture
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Exiled.API.Features;
    using NAudio.Wave;

    /// <inheritdoc/>
    public class VoiceChatRecorder : IVoiceChatRecorder
    {
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public VoiceChatRecorder(Player player)
            : this(new WaveFormat(48000, 1), player)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="waveFormat">The <see cref="VoiceChatCapture"/>.</param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public VoiceChatRecorder(WaveFormat waveFormat, Player player)
        {
            Reset(waveFormat);

            Player = player;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        ~VoiceChatRecorder() => Dispose(false);

        /// <inheritdoc/>
        public WaveFormat WaveFormat { get; }

        /// <summary>
        /// Gets the <see cref="CustomWaveWriter"/> instance.
        /// </summary>
        public CustomWaveWriter Writer { get; private set; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public void Reset(WaveFormat waveFormat)
        {
            Writer?.Dispose();

            Writer = new CustomWaveWriter($@"C:\Users\Pietro\AppData\Roaming\EXILED\Plugins\{DateTime.Now.Ticks}.wav", waveFormat);
        }

        /// <inheritdoc/>
        public void Dispose() => Dispose(true);

        /// <inheritdoc/>
        public async ValueTask WriteAsync(ArraySegment<byte> samples) => await WriteAsync(samples, default);

        /// <inheritdoc/>
        public async ValueTask WriteAsync(ArraySegment<byte> samples, CancellationToken cancellationToken)
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(VoiceChatRecorder));

            await Writer.WriteSamplesAsync(samples);
        }

        /// <summary>
        /// Releases unmanaged resources and, optionally, managed ones.
        /// </summary>
        /// <param name="shouldDisposeAllResources">Indicates whether all resources should be disposed or only unmanaged ones.</param>
        protected virtual void Dispose(bool shouldDisposeAllResources)
        {
            if (shouldDisposeAllResources)
            {
                Writer.Dispose();
                Writer = null;
            }

            isDisposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
