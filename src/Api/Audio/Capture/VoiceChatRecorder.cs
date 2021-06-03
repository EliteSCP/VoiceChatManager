// -----------------------------------------------------------------------
// <copyright file="VoiceChatRecorder.cs" company="iopietro">
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
            : this(player, new WaveFormat(48000, 1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        public VoiceChatRecorder(Player player, WaveFormat waveFormat)
            : this(player, waveFormat, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="rootDirectoryPath"><inheritdoc cref="RootDirectoryPath"/></param>
        public VoiceChatRecorder(Player player, WaveFormat waveFormat, string rootDirectoryPath)
            : this(player, waveFormat, rootDirectoryPath, "dd-MM-yy-HH_mm_ss_fff")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="rootDirectoryPath"><inheritdoc cref="RootDirectoryPath"/></param>
        /// <param name="dateTimeFormat"><inheritdoc cref="DateTimeFormat"/></param>
        public VoiceChatRecorder(Player player, WaveFormat waveFormat, string rootDirectoryPath, string dateTimeFormat)
            : this(player, waveFormat, rootDirectoryPath, dateTimeFormat, 1920)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="rootDirectoryPath"><inheritdoc cref="RootDirectoryPath"/></param>
        /// <param name="dateTimeFormat"><inheritdoc cref="DateTimeFormat"/></param>
        /// <param name="minimumBytesToWrite"><inheritdoc cref="MinimumBytesToWrite"/></param>
        public VoiceChatRecorder(Player player, WaveFormat waveFormat, string rootDirectoryPath, string dateTimeFormat, int minimumBytesToWrite)
            : this(player, waveFormat, rootDirectoryPath, dateTimeFormat, minimumBytesToWrite, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceChatRecorder"/> class.
        /// </summary>
        /// <param name="waveFormat"><inheritdoc cref="WaveFormat"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="rootDirectoryPath"><inheritdoc cref="RootDirectoryPath"/></param>
        /// <param name="dateTimeFormat"><inheritdoc cref="DateTimeFormat"/></param>
        /// <param name="minimumBytesToWrite"><inheritdoc cref="MinimumBytesToWrite"/></param>
        /// <param name="converter"><inheritdoc cref="Converter"/></param>
        public VoiceChatRecorder(Player player, WaveFormat waveFormat, string rootDirectoryPath, string dateTimeFormat, int minimumBytesToWrite, IAudioConverter converter)
        {
            WaveFormat = waveFormat;
            Player = player;
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
        public Player Player { get; }

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
        public CustomWaveWriter Writer { get; private set; }

        /// <inheritdoc/>
        public void Reset()
        {
            if (Writer != null && Writer.Length < MinimumBytesToWrite)
                return;

            var filename = Writer?.Filename;

            Writer?.Dispose();
            Writer = null;

            if (Converter != null && !string.IsNullOrEmpty(filename))
                Converter.StartAsync(filename);
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
                throw new ObjectDisposedException(nameof(VoiceChatRecorder));

            if (Writer == null && Player != null && Player.GameObject != null)
                Writer = new CustomWaveWriter(Path.Combine(RootDirectoryPath, $"{Player.Nickname} ({Player.UserId})", $"({Player.Id}) [{Player.Role}] {DateTime.Now.ToString(DateTimeFormat)}"), WaveFormat);

            await Writer.WriteSamplesAsync(samples);
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
                    Converter.StartAsync(filename);
            }

            isDisposed = true;
        }
    }
}
