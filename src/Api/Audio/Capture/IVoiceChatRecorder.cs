// -----------------------------------------------------------------------
// <copyright file="IVoiceChatRecorder.cs" company="iopietro">
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

    /// <summary>
    /// Records the in-game voice chat.
    /// </summary>
    public interface IVoiceChatRecorder : IDisposable
    {
        /// <summary>
        /// Gets the audio <see cref="WaveFormat"/>, which contains the sample rate and number of channels.
        /// </summary>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> who's been voice recorded.
        /// </summary>
        Player Player { get; }

        /// <summary>
        /// Resets the internal <see cref="Stream"/>.
        /// </summary>
        /// <param name="waveFormat">The new <see cref="WaveFormat"/> to be used.</param>
        void Reset(WaveFormat waveFormat);

        /// <summary>
        /// Writes <see cref="Player"/>'s captured voice samples.
        /// </summary>
        /// <param name="samples">The captured voice samples to write.</param>
        /// <returns>Returns a <see cref="ValueTask"/>.</returns>
        ValueTask WriteAsync(ArraySegment<byte> samples);

        /// <summary>
        /// Writes <see cref="Player"/>'s captured voice samples.
        /// </summary>
        /// <param name="samples">The captured voice samples to write.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the <see cref="Task"/> with.</param>
        /// <returns>Returns a <see cref="ValueTask"/>.</returns>
        ValueTask WriteAsync(ArraySegment<byte> samples, CancellationToken cancellationToken);
    }
}
