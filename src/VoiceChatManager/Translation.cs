// -----------------------------------------------------------------------
// <copyright file="Translation.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager
{
    using Exiled.API.Interfaces;

    /// <summary>
    /// Plugin translation strings.
    /// </summary>
    public class Translation : ITranslation
    {
#pragma warning disable SA1124 // Do not use regions
#pragma warning disable CS1591 // Elements should be documented
#pragma warning disable SA1600 // Elements must be documented
        #region Generic

        public string FailedToRemovePlayerError { get; private set; } = "Failed to remove \"{0}\" ({1}) from the list of voice recorded players!";

        public string FailedToAddPlayerError { get; private set; } = "Failed to add \"{0}\" ({1}) to the list of voice recorded players!";

        public string AudioConverterCannotBeEnabledError { get; private set; } = "Audio converter cannot be enabled, FFmpeg wasn't found at \"{0}\"";

        public string RoundName { get; private set; } = "Round {0}";

        public string SavingGdprConfigsError { get; private set; } = "An error has occurred while saving GDPR configs to {0} path:\n{1}";

        public string LoadingGdprConfigsError { get; private set; } = "An error has occurred while loading GDPR configs from {0} path:\n{1}";

        public string NotEnoughPermissionsError { get; private set; } = "Not enough permissions to run this command!\nRequired: {0}";

        public string InvalidSubCommandError { get; private set; } = "Invalid subcommand! Available: {0}";

        public string CommandIsCurrentlyDisabled { get; private set; } = "Command is currently disabled.";

        public string ExecutingCommandError { get; private set; } = "An error has occurred while executing the command!";

        public string Path { get; private set; } = "Path";

        public string Status { get; private set; } = "Status";

        public string ChannelName { get; private set; } = "Channel name";

        public string Volume { get; private set; } = "Volume";

        public string Duration { get; private set; } = "Duration";

        public string Progression { get; private set; } = "Progression";

        public string Size { get; private set; } = "Size";

        public string Position { get; private set; } = "Position";

        public string Player { get; private set; } = "Player";

        public string MB { get; private set; } = "MB";

        #endregion

        #region Stop Command

        public string StopCommandDescription { get; private set; } = "Stops an audio file from playing.";

        public string StopCommandUsage { get; private set; } = "voicechatmanager stop [Preset name/File name/File path/Audio ID]";

        public string AudioHasBeenStopped { get; private set; } = "Audio \"{0}\" has been stopped.";

        public string AudioNotFoundOrAlreadyStopped { get; private set; } = "Audio \"{0}\" not found or already stopped!";

        #endregion

        #region Play Command

        public string PlayCommandDescription { get; private set; } = "Plays an audio file on a specific channel.";

        public string PlayCommandUsage { get; private set; } = "\nvoicechatmanager play [File alias/File path] [Volume (0-100)]" +
                    "\nvoicechatmanager play [File alias/File path] [Volume (0-100)] [Channel name (SCP, Intercom, Proximity, Ghost)]" +
                    "\nvoicechatmanager play [File alias/File path] [Volume (0-100)] proximity [Player ID/Player Name/Player]" +
                    "\nvoicechatmanager play [File alias/File path] [Volume (0-100)] proximity [X] [Y] [Z]";

        public string ConvertingAudio { get; private set; } = "Converting \"{0}\"...";

        public string FFmpegDirectoryIsNotSetUpProperlyError { get; private set; } = "Your FFmpeg directory isn't set up properly, \"{0}\" won't be converted and played.";

        public string FailedToConvert { get; private set; } = "Failed to convert \"{0}\": {1}";

        public string InvalidVolumeError { get; private set; } = "{0} is not a valid volume, range varies from 0 to 100!";

        public string PlayerNotFoundError { get; private set; } = "Player \"{0}\" not found!";

        public string InvalidCoordinateError { get; private set; } = "\"{0}\" is not a valid {1} coordinate!";

        public string AudioNotFoundOrAlreadyPlaying { get; private set; } = "Audio \"{0}\" not found or it's already playing!";

        public string AudioIsPlayingInAChannel { get; private set; } = "Playing \"{0}\" with {1} volume on \"{2}\" channel, duration: {3}";

        public string AudioIsPlayingNearAPlayer { get; private set; } = "Playing \"{0}\" with {1} volume, in the proximity of \"{2}\", duration: {3}";

        public string AudioIsPlayingInAPosition { get; private set; } = "Playing \"{0}\" with {1} volume, in the proximity of ({2}, {3}, {4}) duration: {5}";

        #endregion

        #region Pause Command

        public string PauseCommandDescription { get; private set; } = "Pause an audio file from playing.";

        public string PauseCommandUsage { get; private set; } = "voicechatmanager pause [Preset name/File name/File path/Audio ID]";

        public string AudioNotFoundOrIsNotPlaying { get; private set; } = "Audio \"{0}\" not found or it's not playing!";

        public string AudioHasBeenPaused { get; private set; } = "Audio \"{0}\" has been paused at {1} ({2}%).";

        #endregion

        #region Forbid Voice Recording Command

        public string ForbidVoiceRecordingCommandDescription { get; private set; } = "Type this command to forbid to be voice recorded for security reasons.";

        public string ForbidVoiceRecordingCommandWarning { get; private set; } = "YOU'RE NOT BEING VOICE RECORDED YET!\nTYPE .acceptvoicerecording TO ACCEPT TO BE VOICE RECORDED FOR SECURITY REASONS.";

        public string YouCannotBeRemovedError { get; private set; } = "An error has occurred! You cannot be removed from the list of voice recorded players!";

        public string YouWontBeVoiceRecordedWarning { get; private set; } = "FROM NOW ON, YOU WON'T BE VOICE RECORDED ANYMORE.";

        #endregion

        #region Accept Voice Recording Command

        public string AcceptVoiceRecordingCommandDescription { get; private set; } = "Type this command to accept to be voice recorded for security reasons.";

        public string AcceptVoiceRecordingCommandWarning { get; private set; } = "\nTYPE THIS COMMAND AGAIN TO ACCEPT TO BE VOICE RECORDED FOR SECURITY REASONS.";

        public string YouCannotBeAddedError { get; private set; } = "An error has occurred! You cannot be added to the list of voice recorded players!";

        public string YourVoiceWillBeRecordedWarning { get; private set; } = "\nFROM NOW ON, YOUR VOICE WILL BE RECORDED FOR SECURITY REASONS, TYPE .disablevoicerecording IF YOU DON'T WANT TO BE VOICE RECORDED ANYMORE.";

        public string AlreadyAcceptedToBeVoiceRecordedError { get; private set; } = "\nYOU'VE ALREADY ACCEPTED TO BE VOICE RECORDED, TYPE .disablevoicerecording IF YOU DON'T WANT TO BE VOICE RECORDED ANYMORE.";

        #endregion

        #region Preset Command

        public string PresetCommandDescription { get; private set; } = "Gets the list of audio presets.";

        public string PresetCommandUsage { get; private set; } = "voicechatmanager pause [Preset name/File name/File path/Audio ID]";

        public string ThereAreNoAudioPresets { get; private set; } = "There are no audio presets.";

        public string AudioPresets { get; private set; } = "Audio presets";

        #endregion

        #region Clear Command

        public string ClearCommandDescription { get; private set; } = "Clears the audios list.";

        public string AudiosListClearedSuccess { get; private set; } = "Audios list cleared successfully!";

        #endregion

        #region Audios Command

        public string AudiosCommandDescription { get; private set; } = "Gets the list of playing/paused/stopped audios.";

        public string AudiosList { get; private set; } = "Audios list";

        public string ThereAreNoAudios { get; private set; } = "There are no audios.";

        #endregion
    }
}
