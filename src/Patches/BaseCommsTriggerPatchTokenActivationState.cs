// -----------------------------------------------------------------------
// <copyright file="BaseCommsTriggerPatchTokenActivationState.cs" company="iopietro">
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
    /// Patches <see cref="BaseCommsTrigger.TokenActivationState"/>.
    /// </summary>
    [HarmonyPatch(typeof(BaseCommsTrigger), nameof(BaseCommsTrigger.TokenActivationState), MethodType.Getter)]
    internal static class BaseCommsTriggerPatchTokenActivationState
    {
        private static bool Prefix(BaseCommsTrigger __instance, ref bool __result)
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
