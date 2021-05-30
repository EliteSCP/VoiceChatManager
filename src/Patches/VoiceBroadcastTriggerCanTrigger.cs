// -----------------------------------------------------------------------
// <copyright file="VoiceBroadcastTriggerCanTrigger.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Patches
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using Dissonance;
    using HarmonyLib;
    using Mirror;
    using RemoteAdmin;

    /// <summary>
    /// Patches <see cref="VoiceBroadcastTrigger.CanTrigger"/>.
    /// </summary>
    [HarmonyPatch(typeof(VoiceBroadcastTrigger), nameof(VoiceBroadcastTrigger.CanTrigger), MethodType.Getter)]
    internal static class VoiceBroadcastTriggerCanTrigger
    {
        private static bool Prefix(VoiceBroadcastTrigger __instance, ref bool __result)
        {
            if (__instance.GetComponent<QueryProcessor>().PlayerId >= 9000 || __instance.GetComponent<NetworkIdentity>().connectionToClient == null)
            {
                __result = true;

                return false;
            }

            return true;
        }
    }
}
