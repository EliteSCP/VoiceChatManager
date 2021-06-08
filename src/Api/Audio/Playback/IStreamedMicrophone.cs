// -----------------------------------------------------------------------
// <copyright file="IStreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Playback
{
    using System;
    using System.IO;
    using Dissonance;
    using Dissonance.Audio.Capture;
    using Enums;

    /// <summary>
    /// A custom streamed microphone.
    /// </summary>
    public interface IStreamedMicrophone : IMicrophoneCapture
    {
        /// <summary>
        /// Gets the microphone capture status.
        /// </summary>
        CaptureStatusType Status { get; }

        /// <summary>
        /// Gets or sets the streamed microphone priority over other source of audio in the same channel.
        /// </summary>
        ChannelPriority Priority { get; set; }

        /// <summary>
        /// Gets the streamed microphone name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the <see cref="DissonanceComms"/> instance.
        /// </summary>
        DissonanceComms DissonanceComms { get; }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the played audio is 3D or not.
        /// </summary>
        bool IsThreeDimensional { get; set; }

        /// <summary>
        /// Gets or sets the audio volume.
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        /// Gets or sets the channel name in which the audio is being played.
        /// </summary>
        string ChannelName { get; set; }

        /// <summary>
        /// Gets the audio <see cref="RoomChannel"/> in which the audio is being played.
        /// </summary>
        RoomChannel RoomChannel { get; }

        /// <summary>
        /// Gets the audio stream  actual progression.
        /// </summary>
        TimeSpan Progression { get; }

        /// <summary>
        /// Gets the audio stream percentage progression.
        /// </summary>
        double PercentageProgression { get; }

        /// <summary>
        /// Gets the volume percentage.
        /// </summary>
        double PercentageVolume { get; }

        /// <summary>
        /// Gets the audio stream duration.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets the audio stream size, in bytes.
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Pauses the microphone capture.
        /// </summary>
        void PauseCapture();

        /// <summary>
        /// Restart the microphone capture.
        /// </summary>
        /// <param name="name">The microphone name.</param>
        /// <param name="isForced">Indicates a value whether the restarting of the capture is forced or not.</param>
        void RestartCapture(string name, bool isForced);

        /// <summary>
        /// Releases all resources.
        /// </summary>
        void Dispose();
    }
}
