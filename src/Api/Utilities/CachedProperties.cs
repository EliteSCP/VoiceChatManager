// -----------------------------------------------------------------------
// <copyright file="CachedProperties.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Utilities
{
    using System;
    using System.Reflection;
    using Api.Audio.Playback;
    using Dissonance;
    using Dissonance.Audio.Codecs;
    using Dissonance.Audio.Playback;
    using Dissonance.Integrations.MirrorIgnorance;
    using Dissonance.Networking;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A set of cached properties.
    /// </summary>
    public static class CachedProperties
    {
        /// <summary>
        /// The dissonance client ID and nickname.
        /// </summary>
        public static readonly (ushort id, string nickname) HostInfo = (9999, "Dummy");

        private static MirrorIgnoranceCommsNetwork commsNetwork;
        private static DissonanceComms dissonanceComms;
        private static ClientInfo<MirrorConn> hostClientInfo;
        private static FieldInfo maximumVoiceChatDesync;

        /// <summary>
        /// Gets the cached mirror ignorance comms network.
        /// </summary>
        public static MirrorIgnoranceCommsNetwork CommsNetwork
        {
            get
            {
                if (commsNetwork == null)
                    commsNetwork = GameObject.FindObjectOfType<MirrorIgnoranceCommsNetwork>();

                return commsNetwork;
            }
        }

        /// <summary>
        /// Gets the cached dissonance comms.
        /// </summary>
        public static DissonanceComms DissonanceComms
        {
            get
            {
                if (dissonanceComms == null)
                    dissonanceComms = GameObject.FindObjectOfType<DissonanceComms>();

                return dissonanceComms;
            }
        }

        /// <summary>
        /// Gets the host client info.
        /// </summary>
        public static ClientInfo<MirrorConn> HostClientInfo
        {
            get
            {
                if (hostClientInfo == null)
                    hostClientInfo = CommsNetwork.Server._clients.GetOrCreateClientInfo(HostInfo.id, HostInfo.nickname, CodecSettings, new MirrorConn(NetworkServer.localConnection));

                return hostClientInfo;
            }
        }

        /// <summary>
        /// Gets or sets the maximum tolerable desync by the decoder pipeline.
        /// </summary>
        public static TimeSpan MaximumVoiceChatDesync
        {
            get => SamplePlaybackComponent.ResetDesync;
            set
            {
                if (maximumVoiceChatDesync == null)
                    maximumVoiceChatDesync = typeof(SamplePlaybackComponent).GetField(nameof(SamplePlaybackComponent.ResetDesync), BindingFlags.Static | BindingFlags.NonPublic);

                maximumVoiceChatDesync.SetValue(null, value);
            }
        }

        /// <summary>
        /// Gets the audio stream codec settings.
        /// </summary>
        public static CodecSettings CodecSettings { get; } = new CodecSettings(Codec.Opus, StreamedMicrophone.FrameSize, 48000);
    }
}
