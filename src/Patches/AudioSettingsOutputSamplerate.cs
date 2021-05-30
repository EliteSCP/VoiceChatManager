// -----------------------------------------------------------------------
// <copyright file="AudioSettingsOutputSamplerate.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Patches
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Sets the project sample rate to 48000 Hz to avoid to throw an exception while trying to create an audio clip.
    /// </summary>
    [HarmonyPatch(typeof(AudioSettings), nameof(AudioSettings.outputSampleRate), MethodType.Getter)]
    internal static class AudioSettingsOutputSamplerate
    {
        private static bool Prefix(ref int __result)
        {
            __result = 48000;
            return false;
        }
    }
}
