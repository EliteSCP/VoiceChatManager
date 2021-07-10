// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using Configs;
    using Core.Audio.Capture;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Plugin main configs.
    /// </summary>
    public class Config : IConfig
    {
        private float volumeLimit = 100f;

        /// <inheritdoc/>
        [Description("Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets <see cref="IVoiceChatRecorder"/> related configs.
        /// </summary>
        [Description("Voice chat recorder related configs")]
        public Recorder Recorder { get; private set; } = new Recorder();

        /// <summary>
        /// Gets <see cref="IAudioConverter"/> related configs.
        /// </summary>
        [Description("Voice chat audio converter related configs")]
        public Converter Converter { get; private set; } = new Converter();

        /// <summary>
        /// Gets a dictionary of preset audios, key is the name, value is the path.
        /// </summary>
        [Description("A dictionary of audio presets with their name as key and path as value")]
        public Dictionary<string, string> Presets { get; private set; } = new Dictionary<string, string>()
        {
            ["windows"] = @"C:\Imposter\sus\AMOGUS.mp3",
            ["linux"] = @"/when/the/imposter/is/sus.mp4",
        };

        /// <summary>
        /// Gets the directory in which ffmpeg.exe (Windows) or ffmpeg (linux) is located.
        /// </summary>
        [Description("The directory in which ffmpeg.exe (Windows) or ffmpeg (linux) is located, leave it empty or null if you don't want to use it")]
        public string FFmpegDirectoryPath { get; private set; }

        /// <summary>
        /// Gets the full path at which GDPR related configs will be saved.
        /// </summary>
        [Description("The full path at which GDPR related configs will be saved")]
        public string GdprConfigFullPath { get; private set; } = Path.Combine(Paths.Configs, "VoiceChatManager", "GDPR", "global.yml");

        /// <summary>
        /// Gets the directory path at which GDPR related configs will be saved.
        /// </summary>
        [YamlIgnore]
        public string GdprConfigDirectoryPath
        {
            get => Path.GetDirectoryName(GdprConfigFullPath);

            // Forced to put this to not make it throw an exception
            private set
            {
            }
        }

        /// <summary>
        /// Gets or sets the volume limit, used for every played audio, from 0 to 100.
        /// </summary>
        [Description("The volume limit, used for every played audio, from 0 to 100")]
        public float VolumeLimit
        {
            get => volumeLimit;
            set
            {
                volumeLimit = Mathf.Clamp(value, 0f, 100f);
            }
        }

        /// <summary>
        /// Gets the audio duration format.
        /// </summary>
        [Description("The audio duration format")]
        public string DurationFormat { get; private set; } = "hh\\:mm\\:ss\\.ff";

        /// <summary>
        /// Gets or sets a value indicating whether the debug is enabled or not.
        /// </summary>
        [Description("Indicates whether the debug is enabled or not")]
        public bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fFMpeg is installed or not.
        /// </summary>
        [YamlIgnore]
        internal bool IsFFmpegInstalled { get; set; }
    }
}
