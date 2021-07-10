// -----------------------------------------------------------------------
// <copyright file="AcceptVoiceRecordingCommand.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Commands
{
    using System;
    using System.IO;
    using CommandSystem;
    using Core.Audio.Capture;
    using Exiled.API.Features;
    using NAudio.Wave;

    /// <inheritdoc/>
    [CommandHandler(typeof(ClientCommandHandler))]
    internal class AcceptVoiceRecordingCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "acceptvoicerecording";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Type this command to accept to be voice recorded for security reasons.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!VoiceChatManager.Instance.Gdpr.IsCompliant || !VoiceChatManager.Instance.Config.Recorder.IsEnabled)
            {
                response = "Command is currently disabled.";
                return false;
            }

            if (!(Player.Get(sender as CommandSender) is Player player))
            {
                response = "An error has occurred while executing the command!";
                return false;
            }

            if (!player.SessionVariables.TryGetValue("canBeVoiceRecorded", out object canBeVoiceRecorded))
            {
                player.SessionVariables.Add("canBeVoiceRecorded", false);

                response = "\nTYPE THIS COMMAND AGAIN TO ACCEPT TO BE VOICE RECORDED FOR SECURITY REASONS.";
                return true;
            }

            if (!(bool)canBeVoiceRecorded)
            {
                player.SessionVariables["canBeVoiceRecorded"] = true;

                var voiceChatRecorder = new VoiceChatRecorder(
                    Talker.GetOrCreate(player.GameObject),
                    new WaveFormat(VoiceChatManager.Instance.Config.Recorder.SampleRate, 1),
                    Path.Combine(VoiceChatManager.Instance.Config.Recorder.RootDirectoryPath, VoiceChatManager.Instance.ServerHandler.RoundName),
                    VoiceChatManager.Instance.Config.Recorder.DateTimeFormat,
                    VoiceChatManager.Instance.Config.Recorder.MinimumBytesToWrite,
                    VoiceChatManager.Instance.Converter);

                voiceChatRecorder.Talker.PlayBackComponent.MultiplyBySource = false;

                if (!VoiceChatManager.Instance.Capture?.Recorders.TryAdd(voiceChatRecorder.Talker, voiceChatRecorder) ?? true)
                {
                    response = "An error has occurred! You cannot be added to the list of voice recorded players!";
                    return true;
                }

                VoiceChatManager.Instance.Gdpr.CanBeVoiceRecordedPlayerUserIds.Add(player.UserId);
                VoiceChatManager.Instance.Gdpr.Save();

                response = "\nFROM NOW ON, YOUR VOICE WILL BE RECORDED FOR SECURITY REASONS, TYPE .disablevoicerecording IF YOU DON'T WANT TO BE VOICE RECORDED ANYMORE.";
                return true;
            }

            response = "\nYOU'VE ALREADY ACCEPTED TO BE VOICE RECORDED, TYPE .disablevoicerecording IF YOU DON'T WANT TO BE VOICE RECORDED ANYMORE.";
            return false;
        }
    }
}
