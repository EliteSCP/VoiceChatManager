// -----------------------------------------------------------------------
// <copyright file="Log.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Logging
{
    using Core.Logging;
    using ExiledLog = Exiled.API.Features.Log;

    /// <summary>
    /// The <see cref="ILog"/> implementation.
    /// </summary>
    internal sealed class Log : ILog
    {
        private readonly bool isDebugEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="isDebugEnabled">Indicates whether the debug is enabled or not.</param>
        public Log(bool isDebugEnabled = false) => this.isDebugEnabled = isDebugEnabled;

        /// <inheritdoc/>
        public void Debug(string message) => ExiledLog.Debug(message, isDebugEnabled);

        /// <inheritdoc/>
        public void Error(string message) => ExiledLog.Error(message);

        /// <inheritdoc/>
        public void Info(string message) => ExiledLog.Info(message);
    }
}
