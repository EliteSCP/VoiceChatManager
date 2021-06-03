// -----------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Api.Audio.Playback;
    using Api.Utilities;
    using Dissonance;
    using Enums;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// A set of extensions to play, pause, stop and get audio streams.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Tries to get a <see cref="IStreamedMicrophone"/> from the <see cref="StreamedMicrophone.List"/>, based on its id.
        /// </summary>
        /// <param name="id">The audio id.</param>
        /// <param name="streamedMicrophone">The audio to get.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> has been found, false otherwise.</returns>
        public static bool TryGet(this int id, out IStreamedMicrophone streamedMicrophone) => StreamedMicrophone.List.TryGet(id, out streamedMicrophone);

        /// <summary>
        /// Tries to get a streamed microphone from the <see cref="StreamedMicrophone.List"/> based on its id (preset name/file name/file path).
        /// </summary>
        /// <param name="id">The streamed microphone id (preset name/file name/file path).</param>
        /// <param name="streamedMicrophone">The streamed microphone to get.</param>
        /// <returns>Returns true if the file is found, false otherwise.</returns>
        public static bool TryGet(this string id, out IStreamedMicrophone streamedMicrophone)
        {
            id.TryGet(out IEnumerable<IStreamedMicrophone> streamedMicrophones);

            return (streamedMicrophone = streamedMicrophones.FirstOrDefault()) != null;
        }

        /// <summary>
        /// Tries to get the first streamed microphone from the <see cref="StreamedMicrophone.List"/> based on its id (preset name/file name/file path) and a filter.
        /// </summary>
        /// <param name="id">The streamed microphone id (preset name/file name/file path).</param>
        /// <param name="status">The streamed microphone status.</param>
        /// <param name="streamedMicrophone">The streamed microphone to get.</param>
        /// <returns>Returns true if the file is found, false otherwise.</returns>
        public static bool TryGet(this string id, CaptureStatusType status, out IStreamedMicrophone streamedMicrophone)
        {
            id.TryGet(status, out IEnumerable<IStreamedMicrophone> streamedMicrophones);

            return (streamedMicrophone = streamedMicrophones.FirstOrDefault()) != null;
        }

        /// <summary>
        /// Tries to get an <see cref="IStreamedMicrophone"/> from the <see cref="StreamedMicrophone.List"/> based on their id (preset name/file name/file path).
        /// </summary>
        /// <param name="id">The streamed microphones id (preset name/file name/file path).</param>
        /// <param name="streamedMicrophones">The <see cref="IStreamedMicrophone"/> to get.</param>
        /// <returns>Returns true if the file is found, false otherwise.</returns>
        public static bool TryGet(this string id, out IEnumerable<IStreamedMicrophone> streamedMicrophones)
        {
            if (VoiceChatManager.Instance.Config.Presets.TryGetValue(id, out var path))
                id = path;

            return (streamedMicrophones = StreamedMicrophone.List.Where(streamedMicrophoneTemp => streamedMicrophoneTemp.Stream != null &&
            streamedMicrophoneTemp.Stream is FileStream fileStream &&
            (fileStream.Name == id || fileStream.Name.Contains(Path.GetFileNameWithoutExtension(id))))).Any();
        }

        /// <summary>
        /// Tries to get an <see cref="IStreamedMicrophone"/> from the <see cref="StreamedMicrophone.List"/> based on their id (preset name/file name/file path) and a filter.
        /// </summary>
        /// <param name="id">The streamed microphones id (preset name/file name/file path).</param>
        /// <param name="status">The streamed microphones status.</param>
        /// <param name="streamedMicrophones">The <see cref="IEnumerable{T}"/> of <see cref="IStreamedMicrophone"/> to get.</param>
        /// <returns>Returns true if the file is found, false otherwise.</returns>
        public static bool TryGet(this string id, CaptureStatusType status, out IEnumerable<IStreamedMicrophone> streamedMicrophones)
        {
            id.TryGet(out streamedMicrophones);

            return (streamedMicrophones = streamedMicrophones.Where(streamedMicrophone => streamedMicrophone.Status == status)).Any();
        }

        /// <summary>
        /// Tries to play an audio file from the <see cref="StreamedMicrophone.List"/>.
        /// </summary>
        /// <param name="id">The <see cref="IStreamedMicrophone"/> id.</param>
        /// <param name="volume">The <see cref="IStreamedMicrophone"/> volume.</param>
        /// <param name="channelName">The channel name in which the audio is tried to be played.</param>
        /// <param name="streamedMicrophone">The <see cref="IStreamedMicrophone"/> to be played.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> started to capture the audio, false otherwise.</returns>
        public static bool TryPlay(this int id, float volume, string channelName, out IStreamedMicrophone streamedMicrophone)
        {
            if (id.TryGet(out streamedMicrophone) && streamedMicrophone.Status != CaptureStatusType.Playing)
            {
                streamedMicrophone.ChannelName = channelName;
                streamedMicrophone.Volume = volume;
                streamedMicrophone.StartCapture(streamedMicrophone.Name);
                return true;
            }

            streamedMicrophone = null;
            return false;
        }

        /// <summary>
        /// Tries to play an audio file in the cache based on its id (preset name/file name/file path).
        /// </summary>
        /// <param name="id">The <see cref="IStreamedMicrophone"/> id (preset name/file name/file path).</param>
        /// <param name="volume">The audio volume (from 0 to 100).</param>
        /// <param name="channelName">The channel name in which the audio is tried to be played.</param>
        /// <param name="streamedMicrophone">The streamed microphone to be started.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> started to capture the audio, false it's already playing or the id isn't valid.</returns>
        public static bool TryPlayInCache(this string id, float volume, string channelName, out IStreamedMicrophone streamedMicrophone)
        {
            if (id.TryGet(CaptureStatusType.Paused, out streamedMicrophone) || id.TryGet(CaptureStatusType.Stopped, out streamedMicrophone))
            {
                streamedMicrophone.ChannelName = channelName;
                streamedMicrophone.Volume = volume;
                streamedMicrophone.StartCapture(streamedMicrophone.Name);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to play an audio file based on its id (preset name/file name/file path).
        /// </summary>
        /// <param name="id">The <see cref="IStreamedMicrophone"/> id (preset name/file name/file path).</param>
        /// <param name="volume">The audio volume (from 0 to 100).</param>
        /// <param name="channelName">The channel name in which the audio is tried to be played.</param>
        /// <param name="streamedMicrophone">The streamed microphone to be started.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> started to capture the audio, false it's already playing or the id isn't valid.</returns>
        public static bool TryPlay(this string id, float volume, string channelName, out IStreamedMicrophone streamedMicrophone)
        {
            if (id.TryPlayInCache(volume, channelName, out streamedMicrophone))
                return true;

            if (!id.TryGet(CaptureStatusType.Playing, out streamedMicrophone) &&
                (File.Exists(id) || (VoiceChatManager.Instance.Config.Presets.TryGetValue(id, out id) && File.Exists(id))))
            {
                return TryPlay(File.OpenRead(id), volume, channelName, out streamedMicrophone);
            }

            return false;
        }

        /// <summary>
        /// Tries to play an audio file based on its id (preset name/file name/file path).
        /// </summary>
        /// <param name="id">The <see cref="IStreamedMicrophone"/> id (preset name/file name/file path).</param>
        /// <param name="position">The <see cref="IStreamedMicrophone"/> proximity position.</param>
        /// <param name="volume">The audio volume (from 0 to 100).</param>
        /// <param name="channelName">The channel name in which the audio is tried to be played.</param>
        /// <param name="streamedMicrophone">The streamed microphone to be started.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> started to capture the audio, false it's already playing or the id isn't valid.</returns>
        public static bool TryPlay(this string id, Vector3 position, float volume, string channelName, out IStreamedMicrophone streamedMicrophone)
        {
            if (id.TryPlayInCache(volume, channelName, out streamedMicrophone))
                return true;

            if (!id.TryGet(CaptureStatusType.Playing, out streamedMicrophone) &&
                (File.Exists(id) || (VoiceChatManager.Instance.Config.Presets.TryGetValue(id, out id) && File.Exists(id))))
            {
                return TryPlay(File.OpenRead(id), position, volume, channelName, out streamedMicrophone);
            }

            return false;
        }

        /// <summary>
        /// Tries to play an audio file based on its id (preset name/file name/file path).
        /// </summary>
        /// <param name="id">The <see cref="IStreamedMicrophone"/> id (preset name/file name/file path).</param>
        /// <param name="player">The <see cref="Player"/> to play the <see cref="IStreamedMicrophone"/> at.</param>
        /// <param name="volume">The audio volume (from 0 to 100).</param>
        /// <param name="channelName">The channel name in which the audio is tried to be played.</param>
        /// <param name="streamedMicrophone">The streamed microphone to be started.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> started to capture the audio, false it's already playing or the id isn't valid.</returns>
        public static bool TryPlay(this string id, Player player, float volume, string channelName, out IStreamedMicrophone streamedMicrophone)
        {
            if (id.TryPlayInCache(volume, channelName, out streamedMicrophone))
                return true;

            if (!id.TryGet(CaptureStatusType.Playing, out streamedMicrophone) &&
                (File.Exists(id) || (VoiceChatManager.Instance.Config.Presets.TryGetValue(id, out id) && File.Exists(id))))
            {
                return TryPlay(File.OpenRead(id), player, volume, channelName, out streamedMicrophone);
            }

            return false;
        }

        /// <summary>
        /// Tries to play an audio <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to be played.</param>
        /// <param name="volume">The audio volume (from 0 to 100).</param>
        /// <param name="channelName">The channel name in which the audio is tried to be played.</param>
        /// <param name="streamedMicrophone">The <see cref="IStreamedMicrophone"/> to be started.</param>
        /// <param name="priority">The audio <see cref="ChannelPriority"/>.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> started to capture the audio, false if the <see cref="Stream"/> is null.</returns>
        public static bool TryPlay(this Stream stream, float volume, string channelName, out IStreamedMicrophone streamedMicrophone, ChannelPriority priority = ChannelPriority.None)
        {
            if (stream == null)
            {
                streamedMicrophone = null;
                return false;
            }

            streamedMicrophone = CachedProperties.DissonanceComms.gameObject.AddComponent<StreamedMicrophone>().Init(stream, volume, channelName, priority);
            streamedMicrophone.RestartCapture(streamedMicrophone.Name);
            return true;
        }

        /// <summary>
        /// Tries to play a proximity audio <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to be played.</param>
        /// <param name="position">The <see cref="IStreamedMicrophone"/> proximity position.</param>
        /// <param name="volume">The audio volume (from 0 to 100).</param>
        /// <param name="channelName">The channel name in which the audio is tried to be played.</param>
        /// <param name="streamedMicrophone">The <see cref="IStreamedMicrophone"/> to be started.</param>
        /// <param name="priority">The audio <see cref="ChannelPriority"/>.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> started to capture the audio, false if the <see cref="Stream"/> is null.</returns>
        public static bool TryPlay(this Stream stream, Vector3 position, float volume, string channelName, out IStreamedMicrophone streamedMicrophone, ChannelPriority priority = ChannelPriority.None)
        {
            if (stream == null)
            {
                streamedMicrophone = null;
                return false;
            }

            streamedMicrophone = CachedProperties.DissonanceComms.gameObject.AddComponent<ProximityStreamedMicrophone>().Init(position, stream, volume, channelName, priority);
            streamedMicrophone.RestartCapture(streamedMicrophone.Name);
            return true;
        }

        /// <summary>
        /// Tries to play an audio <see cref="Stream"/> in the proximity of a <see cref="Player"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to be played.</param>
        /// <param name="player">The <see cref="Player"/> to play the <see cref="IStreamedMicrophone"/> at.</param>
        /// <param name="volume">The audio volume (from 0 to 100).</param>
        /// <param name="channelName">The channel name in which the audio is tried to be played.</param>
        /// <param name="streamedMicrophone">The <see cref="IStreamedMicrophone"/> to be started.</param>
        /// <param name="priority"><inheritdoc cref="IStreamedMicrophone.Priority"/></param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> started to capture the audio, false if the <see cref="Stream"/> is null.</returns>
        public static bool TryPlay(this Stream stream, Player player, float volume, string channelName, out IStreamedMicrophone streamedMicrophone, ChannelPriority priority = ChannelPriority.None)
        {
            if (stream == null || player == null)
            {
                streamedMicrophone = null;
                return false;
            }

            streamedMicrophone = CachedProperties.DissonanceComms.gameObject.AddComponent<PlayerProximityStreamedMicrophone>().Init(player, stream, volume, channelName, priority);
            streamedMicrophone.RestartCapture(streamedMicrophone.Name);
            return true;
        }

        /// <summary>
        /// Stops an <see cref="IStreamedMicrophone"/>, resetting its progression back to the beginning.
        /// </summary>
        /// <param name="id">The <see cref="IStreamedMicrophone"/> id.</param>
        /// <param name="streamedMicrophone">The stopped <see cref="IStreamedMicrophone"/>.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> stopped from capturing the audio, false otherwise.</returns>
        public static bool TryStop(this int id, out IStreamedMicrophone streamedMicrophone)
        {
            if (id.TryGet(out streamedMicrophone) && streamedMicrophone.Status != CaptureStatusType.Stopped)
            {
                streamedMicrophone.StopCapture();
                return true;
            }

            streamedMicrophone = null;
            return false;
        }

        /// <summary>
        /// Stops an <see cref="IStreamedMicrophone"/>, resetting its progression back to the beginning.
        /// </summary>
        /// <param name="id">The <see cref="IStreamedMicrophone"/> id (file name/preset name/file path).</param>
        /// <param name="streamedMicrophone">The stopped <see cref="IStreamedMicrophone"/>.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> stopped from capturing the audio, false otherwise.</returns>
        public static bool TryStop(this string id, out IStreamedMicrophone streamedMicrophone)
        {
            if (id.TryGet(out streamedMicrophone) && streamedMicrophone.Status != CaptureStatusType.Stopped)
            {
                streamedMicrophone.StopCapture();
                return true;
            }

            streamedMicrophone = null;
            return false;
        }

        /// <summary>
        /// Stops every specified <see cref="IStreamedMicrophone"/>.
        /// </summary>
        /// <param name="streamedMicrophones">The <see cref="IEnumerable{T}"/> of <see cref="IStreamedMicrophone"/> to be stopped.</param>
        /// <returns>Returns the amount of stopped <see cref="IStreamedMicrophone"/>.</returns>
        public static uint Stop(this IEnumerable<IStreamedMicrophone> streamedMicrophones)
        {
            var stopped = 0u;

            foreach (var streamedMicrophone in streamedMicrophones)
            {
                if (streamedMicrophone.Status != CaptureStatusType.Stopped)
                {
                    streamedMicrophone.StopCapture();
                    stopped++;
                }
            }

            return stopped;
        }

        /// <summary>
        /// pauses an <see cref="IStreamedMicrophone"/> from capturing.
        /// </summary>
        /// <param name="id">The audio id.</param>
        /// <param name="streamedMicrophone">The paused <see cref="IStreamedMicrophone"/>.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> paused from capturing the audio, false otherwise.</returns>
        public static bool TryPause(this int id, out IStreamedMicrophone streamedMicrophone)
        {
            if (id.TryGet(out streamedMicrophone) && streamedMicrophone.Status == CaptureStatusType.Playing)
            {
                streamedMicrophone.PauseCapture();
                return true;
            }

            streamedMicrophone = null;
            return false;
        }

        /// <summary>
        /// pauses an <see cref="IStreamedMicrophone"/> from capturing.
        /// </summary>
        /// <param name="id">The <see cref="IStreamedMicrophone"/> id (file name/preset name/file path).</param>
        /// <param name="streamedMicrophone">The paused <see cref="IStreamedMicrophone"/>.</param>
        /// <returns>Returns true if the <see cref="IStreamedMicrophone"/> paused from capturing the audio, false otherwise.</returns>
        public static bool TryPause(this string id, out IStreamedMicrophone streamedMicrophone)
        {
            if (id.TryGet(out streamedMicrophone) && streamedMicrophone.Status == CaptureStatusType.Playing)
            {
                streamedMicrophone.PauseCapture();
                return true;
            }

            streamedMicrophone = null;
            return false;
        }

        /// <summary>
        /// Pauses every specified <see cref="IStreamedMicrophone"/>.
        /// </summary>
        /// <param name="streamedMicrophones">The <see cref="IEnumerable{T}"/> of <see cref="IStreamedMicrophone"/> to be paused.</param>
        /// <returns>Returns the amount of paused <see cref="IStreamedMicrophone"/>.</returns>
        public static uint Pause(this IEnumerable<IStreamedMicrophone> streamedMicrophones)
        {
            var paused = 0u;

            foreach (var streamedMicrophone in streamedMicrophones)
            {
                if (streamedMicrophone.Status == CaptureStatusType.Playing)
                {
                    streamedMicrophone.PauseCapture();
                    paused++;
                }
            }

            return paused;
        }

        /// <summary>
        /// Gets the duration of a stream.
        /// </summary>
        /// <param name="stream">The stream to get the duration from.</param>
        /// <param name="frameBytes">The audio frame, in bytes.</param>
        /// <param name="readInterval">The audio read interval.</param>
        /// <returns>Returns the duration of the stream.</returns>
        public static TimeSpan GetDuration(this Stream stream, int frameBytes = 960 * 4, float readInterval = 0.02f) => (stream?.Length ?? 0).GetDuration(frameBytes, readInterval);

        /// <summary>
        /// Gets the duration from the length of a stream.
        /// </summary>
        /// <param name="length">The stream length, in bytes.</param>
        /// <param name="frameBytes">The audio frame, in bytes.</param>
        /// <param name="interval">The audio interval.</param>
        /// <returns>Returns the duration of the stream.</returns>
        public static TimeSpan GetDuration(this long length, int frameBytes = 960 * 4, float interval = 0.02f)
        {
            if (frameBytes == 0)
                throw new DivideByZeroException("frameBytes cannot be 0!");

            return TimeSpan.FromSeconds(length / (float)frameBytes * interval);
        }

        /// <summary>
        /// Converts bytes into megabytes.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <returns>Returns the converted megabytes.</returns>
        public static double FromBytesToMegaBytes(this long bytes) => Math.Round(bytes / 1048576f, 2);
    }
}
