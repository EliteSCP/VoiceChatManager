// -----------------------------------------------------------------------
// <copyright file="PlayerExtensions.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Extensions
{
    using Api.Utilities;
    using Dissonance;
    using Dissonance.Audio.Playback;
    using Dissonance.Integrations.MirrorIgnorance;
    using Exiled.API.Features;

    /// <summary>
    /// <see cref="Player"/> related extensions.
    /// </summary>
    public static class PlayerExtensions
    {
        /// <summary>
        /// Gets the <see cref="VoicePlayback"/> of a specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to get the <see cref="VoicePlayback"/> from.</param>
        /// <param name="voicePlayback">The obtained <see cref="VoicePlayback"/>, if found.</param>
        /// <returns>Returns true if the <see cref="VoicePlayback"/> was found, false otherwise.</returns>
        public static bool TryGet(this Player player, out VoicePlayback voicePlayback)
        {
            voicePlayback = null;
            VoicePlayerState voicePlayerState;

            return player != null
                   && player.GameObject != null
                   && player.GameObject.TryGetComponent<MirrorIgnorancePlayer>(out var mirrorIgnorancePlayer)
                   && (voicePlayerState = CachedProperties.DissonanceComms.FindPlayer(mirrorIgnorancePlayer.PlayerId)) != null
                   && (voicePlayback = (VoicePlayback)voicePlayerState.Playback) != null;
        }

        /// <summary>
        /// Gets the <see cref="SamplePlaybackComponent"/> of a specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to get the <see cref="SamplePlaybackComponent"/> from.</param>
        /// <param name="samplePlaybackComponent">The obtained <see cref="SamplePlaybackComponent"/>, if found.</param>
        /// <returns>Returns true if the <see cref="SamplePlaybackComponent"/> was found, false otherwise.</returns>
        public static bool TryGet(this Player player, out SamplePlaybackComponent samplePlaybackComponent)
        {
            samplePlaybackComponent = null;

            return player.TryGet(out VoicePlayback voicePlayback) && (samplePlaybackComponent = voicePlayback._player) != null;
        }
    }
}
