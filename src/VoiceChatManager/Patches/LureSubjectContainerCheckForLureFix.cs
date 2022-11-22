// -----------------------------------------------------------------------
// <copyright file="LureSubjectContainerCheckForLureFix.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Patches
{
    using HarmonyLib;

    /// <summary>
    /// The server host box collider for the femur breaker shouldn't be spawned.
    /// </summary>
    [HarmonyPatch(typeof(LureSubjectContainer), nameof(LureSubjectContainer.CheckForLure))]
    internal static class LureSubjectContainerCheckForLureFix
    {
        private static bool Prefix() => false;
    }
}
