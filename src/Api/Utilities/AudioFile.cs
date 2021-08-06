namespace VoiceChatManager.Api.Utilities
{
    using System;
    using Api.Extensions;
    using Commands;
    using Exiled.API.Features;
    using UnityEngine;

    public class AudioFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFile"/> class.
        /// </summary>
        public AudioFile()
        {
        }

        public string Name { get; private set; } = string.Empty;

        public float Volume { get; private set; } = 100;

        public string ChannelType { get; private set; } = "Intercom";

        public Vector3 ProximityLocation { get; private set; } = Vector3.zero;

        public void Play()
        {
            if (string.IsNullOrEmpty(Name))
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
