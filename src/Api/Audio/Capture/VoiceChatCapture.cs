// -----------------------------------------------------------------------
// <copyright file="VoiceChatCapture.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Capture
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Extensions;
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
        public void Clear()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            foreach (var recorder in Recorders)
                recorder.Value.Dispose();

            Recorders.Clear();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async Task StartAsync() => await StartAsync(default).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(ReadInterval, cancellationToken).ConfigureAwait(false);

                if (Recorders.IsEmpty)
                    continue;

                await Recorders.ParallelForEachAsync(
                    async recorder =>
                    {
                        if (!recorder.Key.HasActiveSession)
                            return;

                        // TODO: Can be optimized by using ArrayPool
                        var samples = new float[ReadBufferSize];
                        var byteSamples = new byte[ReadBufferSize * 4];

                        try
                        {
                            recorder.Key.OnAudioFilterRead(samples, 1);

                            Buffer.BlockCopy(samples, 0, byteSamples, 0, byteSamples.Length);

                            await recorder.Value.WriteAsync(new ArraySegment<byte>(byteSamples), cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            Log.Error($"[{typeof(VoiceChatCapture).FullName}.{nameof(StartAsync)}] Cannot write voice chat samples of {recorder.Value.Player?.Nickname} ({recorder.Value.Player?.Nickname})!\n{exception}");
                        }
                        finally
                        {
                            if (!recorder.Key.HasActiveSession)
                                recorder.Value.Reset(WaveFormat);
                        }
                    },
                    cancellationToken,
                    Environment.ProcessorCount - 1).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Releases unmanaged resources and, optionally, managed ones.
        /// </summary>
        /// <param name="shouldDisposeAllResources">Indicates whether all resources should be disposed or only unmanaged ones.</param>
        protected virtual void Dispose(bool shouldDisposeAllResources)
        {
            if (isDisposed)
                return;

            if (shouldDisposeAllResources)
                Clear();

            isDisposed = true;
        }
    }
}
