// -----------------------------------------------------------------------
// <copyright file="AudiosCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands.List
{
    using System;
    using System.IO;
    using CommandSystem;
    using Core.Audio.Playback;
    using Core.Extensions;
    using Exiled.Permissions.Extensions;
    using NorthwoodLib.Pools;

    /// <inheritdoc/>
    internal class AudiosCommand : ICommand
    {
        /// <summary>
        /// The command permission.
        /// </summary>
        public const string Permission = "vcm.list.audio";

        private AudiosCommand()
        {
        }

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static AudiosCommand Instance { get; } = new AudiosCommand();

        /// <inheritdoc/>
        public string Command { get; } = "audios";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "audio", "a", "au" };

        /// <inheritdoc/>
        public string Description { get; } = VoiceChatManager.Instance.Translation.AudiosCommandDescription;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(Permission))
            {
                response = string.Format(VoiceChatManager.Instance.Translation.NotEnoughPermissionsError, Permission);
                return false;
            }

            var message = StringBuilderPool.Shared.Rent().AppendLine()
                .Append("[").Append(VoiceChatManager.Instance.Translation.AudiosList).Append(" (").Append(StreamedMicrophone.List.Count).AppendLine(")]")
                .AppendLine();

            if (StreamedMicrophone.List.Count > 0)
            {
                var i = 0;

                foreach (var streamedMicrophone in StreamedMicrophone.List)
                {
                    message.Append("[").Append(i++).Append(". ");

                    if (streamedMicrophone.Stream is FileStream fileStream)
                        message.Append(fileStream.Name);

                    message.AppendLine("]")
                        .Append(VoiceChatManager.Instance.Translation.Status).Append(": ").AppendLine(streamedMicrophone.Status.ToString())
                        .Append(VoiceChatManager.Instance.Translation.ChannelName).Append(": ").AppendLine(streamedMicrophone.ChannelName.ToString())
                        .Append(VoiceChatManager.Instance.Translation.Volume).Append(": ").Append(streamedMicrophone.Volume).Append('/').Append(VoiceChatManager.Instance.Config.VolumeLimit.ToString()).Append(" (").Append(Math.Round(streamedMicrophone.Volume / VoiceChatManager.Instance.Config.VolumeLimit * 100)).AppendLine("%)")
                        .Append(VoiceChatManager.Instance.Translation.Duration).Append(": ").AppendLine(streamedMicrophone.Duration.ToString(VoiceChatManager.Instance.Config.DurationFormat))
                        .Append(VoiceChatManager.Instance.Translation.Progression).Append(": ").Append(streamedMicrophone.Progression.ToString(VoiceChatManager.Instance.Config.DurationFormat)).Append(" (").Append(Math.Round(streamedMicrophone.Stream.Position / (float)streamedMicrophone.Stream.Length * 100f, 1)).AppendLine("%)")
                        .Append(VoiceChatManager.Instance.Translation.Size).Append(": ").Append(streamedMicrophone.Size.FromBytesToMegaBytes()).Append(' ').AppendLine(VoiceChatManager.Instance.Translation.MB);

                    if (streamedMicrophone is IProximityStreamedMicrophone proximityStreamedMicrophone)
                        message.Append(VoiceChatManager.Instance.Translation.Position).Append(": ").AppendLine(proximityStreamedMicrophone.Position.ToString());

                    if (streamedMicrophone is IPlayerProximityStreamedMicrophone playerProximityStreamedMicrophone)
                        message.Append(VoiceChatManager.Instance.Translation.Player).Append(": ").AppendLine(playerProximityStreamedMicrophone.Talker.ToString());

                    message.AppendLine();
                }
            }
            else
            {
                message.Append(VoiceChatManager.Instance.Translation.ThereAreNoAudios);
            }

            response = StringBuilderPool.Shared.ToStringReturn(message);
            return true;
        }
    }
}
