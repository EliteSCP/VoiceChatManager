// -----------------------------------------------------------------------
// <copyright file="RestartTransmissionPipelineFix.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Patches
{
    using Dissonance.Audio.Capture;
    using HarmonyLib;

    /// <summary>
    /// It doesn't restart the transmission pipeline if an audio frame was skipped.
    /// </summary>
    [HarmonyPatch(typeof(CapturePipelineManager), nameof(CapturePipelineManager.RestartTransmissionPipeline))]
    internal static class RestartTransmissionPipelineFix
    {
        private static bool Prefix(string reason) => reason != "Detected a frame skip, forcing capture pipeline reset";
    }
}
