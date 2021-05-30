// -----------------------------------------------------------------------
// <copyright file="RunAsHost.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Patches
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using Dissonance;
    using Dissonance.Integrations.MirrorIgnorance;
    using Dissonance.Networking;
    using HarmonyLib;

    /// <summary>
    /// Runs the server as host instead of dedicated server, to create a local client to stream audio to players.
    /// </summary>
    [HarmonyPatch(typeof(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit>), nameof(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit>.RunAsDedicatedServer))]
    internal static class RunAsHost
    {
        private static bool Prefix(BaseCommsNetwork<MirrorIgnoranceServer, MirrorIgnoranceClient, MirrorConn, Unit, Unit> __instance)
        {
            __instance.RunAsHost(Unit.None, Unit.None);
            return false;
        }
    }
}
