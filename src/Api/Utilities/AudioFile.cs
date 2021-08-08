namespace VoiceChatManager.Api.Utilities
{
    using System;
    using System.IO;
    using Api.Extensions;
    using Commands;
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
        /// Gets the ChannelType on which the audio will be played
        /// </summary>
        public string ChannelType { get; private set; } = "Intercom";

        /// <summary>
        /// Gets the position of played audio, if <see cref="ChannelType"/> is set to "Proximity".
        /// </summary>
        public Vector3 ProximityLocation { get; private set; } = Vector3.zero;

        /// <summary>
        /// Tries to play an audio.
        /// </summary>
        public void Play()
        {
            if (string.IsNullOrEmpty(Name) || !VoiceChatManager.Instance.Config.Presets.ContainsKey(Name) || !File.Exists(Name))
                return;

            string channelType = ChannelType.GetChannelName();

            if (channelType == "Proximity")
            {
                PlayCommand.Instance.Execute(new ArraySegment<string>(new string[] { Name, Volume.ToString(), "proximity", ProximityLocation.x.ToString(), ProximityLocation.y.ToString(), ProximityLocation.z.ToString() }), new ServerConsoleSender(), out string response);
                Log.Debug(response, VoiceChatManager.Instance.Config.IsDebugEnabled);
            }
            else
            {
                PlayCommand.Instance.Execute(new ArraySegment<string>(new string[] { Name, Volume.ToString(), channelType }), new ServerConsoleSender(), out string response);
                Log.Debug(response, VoiceChatManager.Instance.Config.IsDebugEnabled);
            }
        }
    }
}
