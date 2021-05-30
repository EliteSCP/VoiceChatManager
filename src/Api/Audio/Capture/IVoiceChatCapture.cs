// -----------------------------------------------------------------------
// <copyright file="IVoiceChatCapture.cs" company="iopietro">
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
    using Dissonance.Audio.Playback;
    using NAudio.Wave;

    /// <summary>
    /// Captures the in-game voice chat.
    /// </summary>
    public interface IVoiceChatCapture : IDisposable
    {
        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/>, with <see cref="SamplePlaybackComponent"/> as key and <see cref="IVoiceChatRecorder"/> as value.
        /// </summary>
        ConcurrentDictionary<SamplePlaybackComponent, IVoiceChatRecorder> Recorders { get; }

        /// <summary>
        /// Gets the audio <see cref="WaveFormat"/>, which contains the sample rate and number of channels.
        /// </summary>
        WaveFormat WaveFormat { get; }

        /// <summary>
        /// Gets the size of the read buffer which contains voice samples.
        /// </summary>
        int ReadBufferSize { get; }

        /// <summary>
        /// Gets the voice data read interval in milliseconds.
        /// </summary>
        int ReadInterval { get; }

        /// <summary>
        /// Starts the voice recording.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/>.</returns>
        Task StartAsync();

        /// <summary>
        /// Starts the voice recording.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to cancel the <see cref="Task"/> with.</param>
        /// <returns>Returns a <see cref="Task"/>.</returns>
        Task StartAsync(CancellationToken cancellationToken);
    }
}
