// -----------------------------------------------------------------------
// <copyright file="Talker.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Audio.Capture
{
    using System;
    using System.Collections.Generic;
    using Core.Utilities;
    using Dissonance;
    using Dissonance.Audio.Playback;
    using Dissonance.Integrations.MirrorIgnorance;
    using UnityEngine;

    /// <summary>
    /// Represents a player who's speaking.
    /// </summary>
    public class Talker : ITalker
    {
        private ReferenceHub referenceHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="Talker"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="GameObject"/></param>
        private Talker(GameObject gameObject) => ReferenceHub = ReferenceHub.GetHub(gameObject);

        /// <summary>
        /// Gets the talker from a specific gameObject.
        /// </summary>
        public static Dictionary<GameObject, ITalker> GameObjectToTalker { get; } = new Dictionary<GameObject, ITalker>();

        /// <inheritdoc/>
        public string Nickname => ReferenceHub.nicknameSync.Network_myNickSync;

        /// <inheritdoc/>
        public string UserId => referenceHub.characterClassManager.UserId;

        /// <inheritdoc/>
        public int Id => ReferenceHub.queryProcessor.NetworkPlayerId;

        /// <inheritdoc/>
        public RoleType Role => ReferenceHub.characterClassManager.NetworkCurClass;

        /// <inheritdoc/>
        public GameObject GameObject { get; private set; }

        /// <inheritdoc/>
        public ReferenceHub ReferenceHub
        {
            get => referenceHub;
            private set
            {
                if (value == null)
                    throw new ArgumentNullException("ReferenceHub cannot be null!");

                referenceHub = value;
                GameObject = value.gameObject;

                VoicePlayback voicePlayback;
                VoicePlayerState voicePlayerState;

                if (GameObject.TryGetComponent<MirrorIgnorancePlayer>(out var mirrorIgnorancePlayer)
                       && (voicePlayerState = CachedProperties.DissonanceComms.FindPlayer(mirrorIgnorancePlayer.PlayerId)) != null
                       && (voicePlayback = (VoicePlayback)voicePlayerState.Playback) != null)
                {
                    VoicePlayback = voicePlayback;
                    PlayBackComponent = voicePlayback._player;
                }
            }
        }

        /// <inheritdoc/>
        public IVoicePlayback VoicePlayback { get; private set; }

        /// <inheritdoc/>
        public SamplePlaybackComponent PlayBackComponent { get; private set; }

        /// <inheritdoc/>
        public Vector3 Position => ReferenceHub.playerMovementSync.GetRealPosition();

        /// <summary>
        /// Gets or create a new <see cref="Talker"/> instance.
        /// </summary>
        /// <param name="gameObject">The talker's <see cref="GameObject"/>.</param>
        /// <returns>Returns the gotten or created <see cref="ITalker"/>.</returns>
        public static ITalker GetOrCreate(GameObject gameObject)
        {
            if (GameObjectToTalker.TryGetValue(gameObject, out var talker))
                return talker;

            talker = new Talker(gameObject);

            GameObjectToTalker.Add(gameObject, talker);

            return talker;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Nickname} ({UserId}) ({Id}) [{Role}]";
    }
}
