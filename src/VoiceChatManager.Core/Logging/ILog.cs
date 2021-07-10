// -----------------------------------------------------------------------
// <copyright file="ILog.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Logging
{
    /// <summary>
    /// Provides a log contract.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// The error level log.
        /// </summary>
        /// <param name="message">The log message to be shown.</param>
        void Error(string message);

        /// <summary>
        /// The info level log.
        /// </summary>
        /// <param name="message">The log message to be shown.</param>
        void Info(string message);

        /// <summary>
        /// The debug level log.
        /// </summary>
        /// <param name="message">The log message to be shown.</param>
        void Debug(string message);
    }
}
