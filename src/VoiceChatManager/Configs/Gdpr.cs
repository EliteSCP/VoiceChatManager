// -----------------------------------------------------------------------
// <copyright file="Gdpr.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Configs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Loader;
    using static VoiceChatManager;

    /// <summary>
    /// GDPR related configs.
    /// </summary>
    public sealed class Gdpr
    {
        /// <summary>
        /// Gets or sets a value indicating whether the European Union GDPR should be respected or not.
        /// </summary>
        [Description("Indicates whether the European Union GDPR should be respected or not")]
        public bool IsCompliant { get; set; }

        /// <summary>
        /// Gets the hashset of players who have accepted to be be voice recorded, based on their Steam ID/Discord ID.
        /// </summary>
        [Description("The list of players who have accepted to be be voice recorded, based on their Steam ID/Discord ID")]
        public HashSet<string> CanBeVoiceRecordedPlayerUserIds { get; private set; } = new HashSet<string>();

        /// <summary>
        /// Saves GDPR related configs.
        /// </summary>
        public void Save()
        {
            try
            {
                File.WriteAllText(Instance.Config.GdprConfigFullPath, ConfigManager.Serializer.Serialize(this));
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while saving GDPR configs to {Instance.Config.GdprConfigFullPath} path:\n{exception}");
            }
        }

        /// <summary>
        /// Loads GDPR related configs.
        /// </summary>
        public void Load()
        {
            try
            {
                if (!Directory.Exists(Instance.Config.GdprConfigDirectoryPath))
                    Directory.CreateDirectory(Instance.Config.GdprConfigDirectoryPath);

                if (!File.Exists(Instance.Config.GdprConfigFullPath))
                {
                    File.Create(Instance.Config.GdprConfigFullPath).Close();

                    Save();
                    return;
                }

                this.CopyProperties(ConfigManager.Deserializer.Deserialize<Gdpr>(File.ReadAllText(Instance.Config.GdprConfigFullPath)));
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while loading GDPR configs from {Instance.Config.GdprConfigFullPath} path:\n{exception}");
            }
        }
    }
}
