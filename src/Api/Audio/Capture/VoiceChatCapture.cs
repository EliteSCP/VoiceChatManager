// -----------------------------------------------------------------------
// <copyright file="VoiceChatCapture.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Capture
{
    using System;
    using System.Buffers;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Dissonance.Audio.Playback;
    using Exiled.API.Features;
    using NAudio.Wave;

    /// <inheritdoc/>
    public class VoiceChatCapture : IVoiceChatCapture
    {
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatCapture"/> class.
        /// </summary>
        public VoiceChatCapture()
            : this(new WaveFormat(48000, 1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatCapture"/> class.
        /// </summary>
        /// <param name="waveFormat">The <see cref="WaveFormat"/>, which contains the sample rate and number of channels.</param>
        public VoiceChatCapture(WaveFormat waveFormat)
            : this(waveFormat, 1920)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatCapture"/> class.
        /// </summary>
        /// <param name="waveFormat">The <see cref="WaveFormat"/>, which contains the sample rate and number of channels.</param>
        /// <param name="readBufferSize"><inheritdoc cref="ReadBufferSize"/></param>
        public VoiceChatCapture(WaveFormat waveFormat, int readBufferSize)
            : this(waveFormat, readBufferSize, 40)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatCapture"/> class.
        /// </summary>
        /// <param name="waveFormat">The <see cref="WaveFormat"/>, which contains the sample rate and number of channels.</param>
        /// <param name="readBufferSize"><inheritdoc cref="ReadBufferSize"/></param>
        /// <param name="readInterval"><inheritdoc cref="ReadInterval"/></param>
        public VoiceChatCapture(WaveFormat waveFormat, int readBufferSize, int readInterval)
        {
            WaveFormat = waveFormat ?? throw new ArgumentNullException(nameof(WaveFormat));
            ReadBufferSize = Math.Max(readBufferSize, 960);
            ReadInterval = Math.Max(readInterval, 20);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="VoiceChatCapture"/> class.
        /// </summary>
        ~VoiceChatCapture() => Dispose(false);

        /// <inheritdoc/>
        public WaveFormat WaveFormat { get; }

        /// <inheritdoc/>
        public ConcurrentDictionary<SamplePlaybackComponent, IVoiceChatRecorder> Recorders { get; } = new ConcurrentDictionary<SamplePlaybackComponent, IVoiceChatRecorder>();

        /// <inheritdoc/>
        public int ReadBufferSize { get; }

        /// <inheritdoc/>
        public int ReadInterval { get; }

        /// <inheritdoc/>
        public void Dispose() => Dispose(true);

        /// <inheritdoc/>
        public async Task StartAsync() => await StartAsync(default);

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            await ReadAsync(cancellationToken);
        }

        /// <summary>
        /// Releases unmanaged resources and, optionally, managed ones.
        /// </summary>
        /// <param name="shouldDisposeAllResources">Indicates whether all resources should be disposed or only unmanaged ones.</param>
        protected virtual void Dispose(bool shouldDisposeAllResources)
        {
            if (shouldDisposeAllResources)
            {
                foreach (var recorder in Recorders.Values)
                    recorder.Dispose();

                Recorders.Clear();
            }

            isDisposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Updates the read thread.
        /// </summary>
        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            var parallelOptions = new ParallelOptions()
            {
                CancellationToken = cancellationToken,
            };

            while (true)
            {
                Parallel.ForEach(Recorders, parallelOptions, async recorder =>
                {
                    if (!recorder.Key.HasActiveSession)
                        return;

                    var samples = ArrayPool<float>.Shared.Rent(ReadBufferSize);
                    var byteSamples = ArrayPool<byte>.Shared.Rent(ReadBufferSize * 4);

                    try
                    {
                        recorder.Key.OnAudioFilterRead(samples, 1);

                        Buffer.BlockCopy(samples, 0, byteSamples, 0, byteSamples.Length);

                        await recorder.Value.WriteAsync(new ArraySegment<byte>(byteSamples), cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"[{typeof(VoiceChatCapture).FullName}.{nameof(ReadAsync)}] Cannot write voice chat samples of {recorder.Value.Player?.Nickname} ({recorder.Value.Player?.Nickname})!\n{exception}");
                    }
                    finally
                    {
                        ArrayPool<float>.Shared.Return(samples, true);
                        ArrayPool<byte>.Shared.Return(byteSamples, true);

                        if (!recorder.Key.HasActiveSession)
                            recorder.Value.Reset(WaveFormat);
                    }
                });

                await Task.Delay(ReadInterval, cancellationToken);
            }
        }
    }
}
