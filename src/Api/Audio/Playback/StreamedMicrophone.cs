// -----------------------------------------------------------------------
// <copyright file="StreamedMicrophone.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Audio.Playback
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Dissonance;
    using Dissonance.Audio.Capture;
    using Enums;
    using Extensions;
    using NAudio.Wave;
    using UnityEngine;
    using static VoiceChatManager;
    using Log = Exiled.API.Features.Log;

    /// <inheritdoc cref="IStreamedMicrophone"/>
    /// <remarks>Based on https://gist.github.com/martindevans/ad4df4d1f771538bb7f474756cbb3711 .</remarks>
    public class StreamedMicrophone : MonoBehaviour, IStreamedMicrophone, IDisposable
    {
        /// <summary>
        /// The size of the audio frame.
        /// </summary>
        public const int FrameSize = 1920;

        /// <summary>
        /// The audio sample rate.
        /// </summary>
        public const int SampleRate = 48000;

        private readonly WaveFormat format = new WaveFormat(SampleRate, 1);
        private readonly float[] frame = new float[FrameSize];
        private readonly byte[] frameBytes = new byte[FrameSize * 4];
        private readonly List<IMicrophoneSubscriber> subscribers = new List<IMicrophoneSubscriber>();
        private DissonanceComms dissonanceComms;
        private float elapsedTime;
        private bool isDisposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="StreamedMicrophone"/> class.
        /// </summary>
        ~StreamedMicrophone() => Dispose(false);

        /// <summary>
        /// Gets the <see cref="List{T}"/> of playing/paused/stopped <see cref="IStreamedMicrophone"/>.
        /// </summary>
        public static List<IStreamedMicrophone> List { get; private set; } = new List<IStreamedMicrophone>();

        /// <inheritdoc/>
        public virtual CaptureStatusType Status { get; protected set; }

        /// <inheritdoc/>
        public ChannelPriority Priority { get; set; }

        /// <inheritdoc/>
        public virtual string Name { get; protected set; } = "StreamedMicrophone";

        /// <summary>
        /// Gets the <see cref="DissonanceComms"/> instance.
        /// </summary>
        public DissonanceComms DissonanceComms
        {
            get
            {
                if (dissonanceComms == null)
                    dissonanceComms = GetComponent<DissonanceComms>();

                return dissonanceComms;
            }
        }

        /// <inheritdoc/>
        public Stream Stream { get; protected set; }

        /// <inheritdoc/>
        public virtual bool IsRecording { get; protected set; }

        /// <inheritdoc/>
        public virtual bool IsThreeDimensional { get; set; }

        /// <inheritdoc/>
        public TimeSpan Latency { get; protected set; }

        /// <inheritdoc/>
        public virtual float Volume { get; set; }

        /// <inheritdoc/>
        public double PercentageVolume => Math.Round(Volume / Instance.Config.VolumeLimit * 100);

        /// <inheritdoc/>
        public virtual string ChannelName { get; set; }

        /// <inheritdoc/>
        public virtual RoomChannel RoomChannel { get; private set; }

        /// <inheritdoc/>
        public TimeSpan Duration { get; protected set; }

        /// <inheritdoc/>
        public TimeSpan Progression => Stream.Position.GetDuration();

        /// <inheritdoc/>
        public double PercentageProgression => Math.Round(Stream.Position / (float)Stream.Length * 100f, 1);

        /// <inheritdoc/>
        public long Size { get; protected set; }

        /// <summary>
        /// Inits the class.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <param name="volume"><inheritdoc cref="Volume"/></param>
        /// <param name="channelname"><inheritdoc cref="ChannelName"/></param>
        /// <param name="priority"><inheritdoc cref="Priority"/></param>
        /// <returns>Returns the class instance.</returns>
        public virtual IStreamedMicrophone Init(Stream stream, float volume, string channelname, ChannelPriority priority = ChannelPriority.None)
        {
            Stream = stream ?? throw new ArgumentNullException("Stream cannot be null!");
            Volume = Mathf.Clamp(volume, 0, Instance.Config.VolumeLimit);
            ChannelName = channelname;
            Priority = priority;
            Duration = Stream.GetDuration();
            Size = Stream.Length;

            List.Add(this);

            return this;
        }

        /// <summary>
        /// Starts to capture a specified audio file.
        /// </summary>
        /// <param name="name">The microphone name.</param>
        /// <returns>Returns the <see cref="WaveFormat"/>.</returns>
        public virtual WaveFormat StartCapture(string name)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else if (Stream == null)
            {
                Log.Error($"Stream is null! Microphone name: \"{name}\".");
                return format;
            }
            else if (!Stream.CanRead)
            {
                Log.Error($"Stream cannot be read! Microphone name: \"{name}\".");
                return format;
            }

            List.Pause();

            RoomChannel = DissonanceComms.RoomChannels.Open(ChannelName, IsThreeDimensional, Priority, Volume / 100);

            elapsedTime = 0;

            DissonanceComms._capture._micName = Name = string.IsNullOrEmpty(name) ? Name : name;
            DissonanceComms._capture._microphone = this;

            IsRecording = true;
            Status = CaptureStatusType.Playing;
            DissonanceComms.IsMuted = false;

            Log.Debug($"Stream of duration {Stream.GetDuration().ToString(Instance.Config.DurationFormat)} started. Microphone name: \"{name}\", is 3D: {(IsThreeDimensional ? "Yes" : "No")}, channel name: {ChannelName}, priority: {Priority}.");

            return format;
        }

        /// <inheritdoc/>
        public virtual void StopCapture()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!EqualityComparer<RoomChannel>.Default.Equals(RoomChannel, default))
                DissonanceComms.RoomChannels.Close(RoomChannel);

            IsRecording = false;
            Status = CaptureStatusType.Stopped;

            Log.Debug($"Stream has been stopped at {Stream.Position.GetDuration().ToString(Instance.Config.DurationFormat)}.", Instance.Config.IsDebugEnabled);

            if (Stream?.CanSeek ?? false)
                Stream.Seek(0, SeekOrigin.Begin);
        }

        /// <inheritdoc/>
        public virtual void PauseCapture()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!EqualityComparer<RoomChannel>.Default.Equals(RoomChannel, default))
                DissonanceComms.RoomChannels.Close(RoomChannel);

            IsRecording = false;
            Status = CaptureStatusType.Paused;

            Log.Debug($"Stream has been paused at {Stream.Position.GetDuration().ToString(Instance.Config.DurationFormat)}.", Instance.Config.IsDebugEnabled);
        }

        /// <inheritdoc/>
        public virtual void RestartCapture(string name, bool force = true)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (force)
                StopCapture();

            if (DissonanceComms._capture._network == null)
                DissonanceComms._capture._network = DissonanceComms._net;

            DissonanceComms._capture._micName = Name = string.IsNullOrEmpty(name) ? Name : name;
            DissonanceComms._capture._microphone = this;

            DissonanceComms.ResetMicrophoneCapture();
        }

        /// <inheritdoc/>
        public void Subscribe(IMicrophoneSubscriber listener) => subscribers.Add(listener);

        /// <inheritdoc/>
        public bool Unsubscribe(IMicrophoneSubscriber listener) => subscribers.Remove(listener);

        /// <inheritdoc/>
        public virtual bool UpdateSubscribers()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (Stream == null)
            {
                StopCapture();
                return true;
            }

            elapsedTime += Time.unscaledDeltaTime;

            while (elapsedTime > 0.04f)
            {
                elapsedTime -= 0.04f;

                var readLength = Stream.Read(frameBytes, 0, frameBytes.Length);

                Array.Clear(frame, 0, frame.Length);

                Buffer.BlockCopy(frameBytes, 0, frame, 0, readLength);

                foreach (var subscriber in subscribers)
                    subscriber.ReceiveMicrophoneData(new ArraySegment<float>(frame), format);
            }

            if (Stream.Position == Stream.Length)
                StopCapture();

            return false;
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and, optionally, managed ones.
        /// </summary>
        /// <param name="shouldDisposeAllResources">Indicates whether all resources should be disposed or only unmanaged ones.</param>
        protected virtual void Dispose(bool shouldDisposeAllResources)
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            StopCapture();

            if (shouldDisposeAllResources)
            {
                Stream?.Dispose();
                Stream = null;
            }

            isDisposed = true;

            Destroy(this);
        }
    }
}
