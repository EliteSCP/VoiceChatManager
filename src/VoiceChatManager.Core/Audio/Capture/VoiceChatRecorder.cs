// -----------------------------------------------------------------------
// <copyright file="VoiceChatRecorder.cs" company="iopietro">
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

    /// <inheritdoc/>
    public class VoiceChatRecorder : IVoiceChatRecorder
    {
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="talker"><inheritdoc cref="Talker"/></param>
        public VoiceChatRecorder(ITalker talker)
            : this(talker, new WaveFormat(48000, 1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="talker"><inheritdoc cref="Talker"/></param>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        public VoiceChatRecorder(ITalker talker, WaveFormat waveFormat)
            : this(talker, waveFormat, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="talker"><inheritdoc cref="Talker"/></param>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="rootDirectoryPath"><inheritdoc cref="RootDirectoryPath"/></param>
        public VoiceChatRecorder(ITalker talker, WaveFormat waveFormat, string rootDirectoryPath)
            : this(talker, waveFormat, rootDirectoryPath, "dd-MM-yy-HH_mm_ss_fff")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="talker"><inheritdoc cref="Talker"/></param>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="rootDirectoryPath"><inheritdoc cref="RootDirectoryPath"/></param>
        /// <param name="dateTimeFormat"><inheritdoc cref="DateTimeFormat"/></param>
        public VoiceChatRecorder(ITalker talker, WaveFormat waveFormat, string rootDirectoryPath, string dateTimeFormat)
            : this(talker, waveFormat, rootDirectoryPath, dateTimeFormat, 1920)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="talker"><inheritdoc cref="Talker"/></param>
        /// <param name="rootDirectoryPath"><inheritdoc cref="RootDirectoryPath"/></param>
        /// <param name="dateTimeFormat"><inheritdoc cref="DateTimeFormat"/></param>
        /// <param name="minimumBytesToWrite"><inheritdoc cref="MinimumBytesToWrite"/></param>
        public VoiceChatRecorder(ITalker talker, WaveFormat waveFormat, string rootDirectoryPath, string dateTimeFormat, int minimumBytesToWrite)
            : this(talker, waveFormat, rootDirectoryPath, dateTimeFormat, minimumBytesToWrite, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="talker"><inheritdoc cref="Talker"/></param>
        /// <param name="rootDirectoryPath"><inheritdoc cref="RootDirectoryPath"/></param>
        /// <param name="dateTimeFormat"><inheritdoc cref="DateTimeFormat"/></param>
        /// <param name="minimumBytesToWrite"><inheritdoc cref="MinimumBytesToWrite"/></param>
        /// <param name="converter"><inheritdoc cref="Converter"/></param>
        public VoiceChatRecorder(ITalker talker, WaveFormat waveFormat, string rootDirectoryPath, string dateTimeFormat, int minimumBytesToWrite, IAudioConverter converter)
        {
            WaveFormat = waveFormat;
            Talker = talker;
            RootDirectoryPath = rootDirectoryPath;
            DateTimeFormat = dateTimeFormat;
            MinimumBytesToWrite = minimumBytesToWrite;
            Converter = converter;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        ~VoiceChatRecorder() => Dispose(false);

        /// <inheritdoc/>
        public ITalker Talker { get; }

        /// <inheritdoc/>
        public WaveFormat WaveFormat { get; set; }

        /// <inheritdoc/>
        public string RootDirectoryPath { get; set; }

        /// <inheritdoc/>
        public string DateTimeFormat { get; set; }

        /// <inheritdoc/>
        public int MinimumBytesToWrite { get; set; }

        /// <inheritdoc/>
        public IAudioConverter Converter { get; }

        /// <inheritdoc/>
        public ICustomWaveWriter Writer { get; private set; }

        /// <inheritdoc/>
        public void Reset() => Reset(WaveFormat);

        /// <inheritdoc/>
        public void Reset(WaveFormat waveFormat)
        {
            if (Writer != null && Writer.Length < MinimumBytesToWrite)
                return;

            var filename = Writer?.Filename;

            Writer?.Dispose();
            Writer = null;

            if (Converter != null && !string.IsNullOrEmpty(filename))
                Converter.Queue.Enqueue(filename);

            WaveFormat = waveFormat;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async ValueTask WriteAsync(ArraySegment<byte> samples) => await WriteAsync(samples, default);

        /// <inheritdoc/>
        public async ValueTask WriteAsync(ArraySegment<byte> samples, CancellationToken cancellationToken)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            cancellationToken.ThrowIfCancellationRequested();

            if (Writer == null && Talker != null && Talker.GameObject != null)
                Writer = new CustomWaveWriter(Path.Combine(RootDirectoryPath, $"{Talker.Nickname} ({Talker.UserId})", $"({Talker.Id}) [{Talker.Role}] {DateTime.Now.ToString(DateTimeFormat)}"), WaveFormat);

            await Writer.WriteSamplesAsync(samples, cancellationToken).ConfigureAwait(false);
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
            {
                var filename = Writer?.Filename;

                Writer?.Dispose();
                Writer = null;

                if (Converter != null && !string.IsNullOrEmpty(filename))
                    Converter.Queue.Enqueue(filename);
            }

            isDisposed = true;
        }
    }
}
