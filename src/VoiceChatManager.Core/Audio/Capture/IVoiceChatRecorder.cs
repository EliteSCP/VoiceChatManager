// -----------------------------------------------------------------------
// <copyright file="IVoiceChatRecorder.cs" company="iopietro">
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
    /// Records the in-game voice chat.
    /// </summary>
    public interface IVoiceChatRecorder : IDisposable
    {
        /// <summary>
        /// Gets or sets the audio <see cref="WaveFormat"/>, which contains the sample rate and number of channels.
        /// </summary>
        WaveFormat WaveFormat { get; set; }

        /// <summary>
        /// Gets the <see cref="ITalker"/> who's been voice recorded.
        /// </summary>
        ITalker Talker { get; }

        /// <summary>
        /// Gets the root directory path, at which audio files will be saved.
        /// </summary>
        string RootDirectoryPath { get; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> format that will be written in the file name.
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings .</remarks>
        string DateTimeFormat { get; }

        /// <summary>
        /// Gets the minimum number of bytes required to write the audio, minimum is the read buffer size.
        /// </summary>
        int MinimumBytesToWrite { get; }

        /// <summary>
        /// Gets the audio converter.
        /// </summary>
        IAudioConverter Converter { get; }

        /// <summary>
        /// Gets the <see cref="ICustomWaveWriter"/> instance.
        /// </summary>
        ICustomWaveWriter Writer { get; }

        /// <summary>
        /// Resets the internal <see cref="Stream"/>.
        /// </summary>
        void Reset();

        /// <summary>
        /// Resets the internal <see cref="Stream"/> and change the <see cref="WaveFormat"/> with the one provided.
        /// </summary>
        /// <param name="waveFormat">The new <see cref="WaveFormat"/> to set.</param>
        void Reset(WaveFormat waveFormat);

        /// <summary>
        /// Writes <see cref="Talker"/>'s captured voice samples.
        /// </summary>
        /// <param name="samples">The captured voice samples to write.</param>
        /// <returns>Returns a <see cref="ValueTask"/>.</returns>
        ValueTask WriteAsync(ArraySegment<byte> samples);

        /// <summary>
        /// Writes <see cref="Talker"/>'s captured voice samples.
        /// </summary>
        /// <param name="samples">The captured voice samples to write.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the <see cref="Task"/> with.</param>
        /// <returns>Returns a <see cref="ValueTask"/>.</returns>
        ValueTask WriteAsync(ArraySegment<byte> samples, CancellationToken cancellationToken);
    }
}
