// -----------------------------------------------------------------------
// <copyright file="ServerHostProfile.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Audio.Profiles
{
    using Assets._Scripts.Dissonance;

    /// <summary>
    /// Represents the server host voice chat profile.
    /// </summary>
    public class ServerHostProfile : VoiceProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHostProfile"/> class.
        /// </summary>
        /// <param name="dissonanceSetup">The dissonance setup.</param>
        public ServerHostProfile(DissonanceUserSetup dissonanceSetup)
            : base(dissonanceSetup)
        {
        }

        /// <inheritdoc/>
        public override void Apply()
        {
            dissonanceSetup.ResetToDefault();

            dissonanceSetup.EnableSpeaking(TriggerType.Proximity | TriggerType.Role | TriggerType.Intercom, RoleType.Null);
            dissonanceSetup.EnableListening(TriggerType.Proximity | TriggerType.Role | TriggerType.Intercom, RoleType.Ghost);
        }
    }
}
