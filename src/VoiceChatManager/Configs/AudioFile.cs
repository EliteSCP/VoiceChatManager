// -----------------------------------------------------------------------
// <copyright file="AudioFile.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Configs
{
    using System;
    using System.Collections.Generic;
    using Commands;
    using Core.Extensions;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Used for handling playing audio on certain event.
    /// </summary>
    public class AudioFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFile"/> class.
        /// </summary>
        public AudioFile()
        {
        }

        /// <summary>
        /// Gets the name of the preset or path to the file.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the volume of the audio.
        /// </summary>
        public float Volume { get; private set; } = 100;

        /// <summary>
        /// Gets the ChannelType on which the audio will be played.
        /// </summary>
        public string ChannelType { get; private set; } = "Intercom";

        /// <summary>
        /// Gets the position of played audio, if <see cref="ChannelType"/> is set to "Proximity".
        /// </summary>
        public Vector3 ProximityLocation { get; private set; } = Vector3.zero;

        /// <summary>
        /// Tries to play an audio.
        /// </summary>
        /// <returns>Returns a value indicating whether the audio is being played or not.</returns>
        public bool Play()
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            string channelType = ChannelType.GetChannelName();
            List<string> arguments = new List<string>()
            {
                Name,
                Volume.ToString(),
                channelType,
            };

            if (channelType == "Proximity")
            {
                arguments.AddRange(new string[]
                {
                    ProximityLocation.x.ToString(),
                    ProximityLocation.y.ToString(),
                    ProximityLocation.z.ToString(),
                });
            }

            var isBeingPlayed = PlayCommand.Instance.Execute(new ArraySegment<string>(arguments.ToArray()), new ServerConsoleSender(), out string response);

            Log.Debug(response, VoiceChatManager.Instance.Config.IsDebugEnabled);

            return isBeingPlayed;
        }
    }
}
