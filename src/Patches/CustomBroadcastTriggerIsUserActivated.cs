// -----------------------------------------------------------------------
// <copyright file="CustomBroadcastTriggerIsUserActivated.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Patches
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;
    using Mirror;
    using RemoteAdmin;

    /// <summary>
    /// Patches <see cref="CustomBroadcastTrigger.IsUserActivated"/>.
    /// </summary>
    [HarmonyPatch(typeof(CustomBroadcastTrigger), nameof(CustomBroadcastTrigger.IsUserActivated))]
    internal static class CustomBroadcastTriggerIsUserActivated
    {
        private static bool Prefix(CustomBroadcastTrigger __instance, ref bool __result)
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
