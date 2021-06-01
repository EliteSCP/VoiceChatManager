// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Api.Extensions
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Exiled.API.Features;
    using Xabe.FFmpeg;

    /// <summary>
    /// <see cref="string"/> related extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the channel name from a raw one.
        /// </summary>
        /// <param name="rawChannelName">The raw channel name.</param>
        /// <returns>Returns the obtained channel name.</returns>
        public static string GetChannelName(this string rawChannelName)
        {
            switch (rawChannelName.ToLower())
            {
                default:
                case "intercom":
                    return "Intercom";

                case "p":
                case "proximity":
                    return "Proximity";

                case "spec":
                case "spectators":
                case "spectator":
                case "ghost":
                    return "Ghost";

                case "scp":
                    return "SCP";
            }
        }

        /// <summary>
        /// Replaces all invalid chars from a specified path, with a specified char.
        /// </summary>
        /// <param name="path">The path to remove all invalid chars from.</param>
        /// <param name="replacer">The char that will replace all invalid chars.</param>
        /// <returns>Returns the sanitized path.</returns>
        public static string ReplaceIllegalPathCharacters(this string path, char replacer = char.MinValue)
        {
            foreach (var @char in new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()))
                path = path.Replace(@char, replacer);

            return path;
        }

        /// <summary>
        /// Converts an audio/video file to a specified format.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="sampleRate">The converted audio sample rate.</param>
        /// <param name="channels">The converted audio channels.</param>
        /// <param name="speed">The converted audio speed.</param>
        /// <param name="format">The converted audio format.</param>
        /// <param name="canOverwriteOutput">Indicates whether the output file can be overwritten or not.</param>
        /// <returns>Returns a <see cref="Task{TResult}"/>.</returns>
        public static async Task<IConversionResult> ConvertFileAsync(this string path, int sampleRate = 48000, int channels = 1, float speed = 1, Format format = Format.f32le, bool canOverwriteOutput = true)
        {
            Log.Debug($"Converting \"{path}\"", VoiceChatManager.Instance.Config.IsDebugEnabled);

            var audioStream = (await FFmpeg.GetMediaInfo(path))?.AudioStreams?.FirstOrDefault();

            if (audioStream == null)
                throw new InvalidDataException($"File \"{path}\" doesn't contain audio tracks!");

            var result = await FFmpeg.Conversions.New()
                .AddStream(audioStream)
                .AddParameter($"-ar {sampleRate} -ac {channels} -filter:a \"atempo = {speed}\"")
                .SetOutput($"{path}.raw")
                .SetOutputFormat(format)
                .SetOverwriteOutput(canOverwriteOutput)
                .Start();

            Log.Debug($"Converted \"{path}\" successfully in {result.Duration}, with {result.Arguments} as arguments, starting time: {result.StartTime}, ending time: {result.EndTime}.");

            return result;
        }

        /// <summary>
        /// Gets a valid file name, creating all folders if necessary.
        /// </summary>
        /// <param name="path">The file path to be validated.</param>
        /// <param name="extension">The optional extension to append at the end of the filename.</param>
        /// <returns>Returns the validated filename.</returns>
        public static string GetValidFilePath(this string path, string extension = null)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var directoryPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (!string.IsNullOrEmpty(extension) && !path.EndsWith(extension))
                path += extension;

            return path;
        }
    }
}
