// -----------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Extensions
{
    using System;

    /// <summary>
    /// DateTime related extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Applies a TimeZone into a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateTime">The chosen <see cref="DateTime"/>.</param>
        /// <param name="timeZone">The TimeZone to apply.</param>
        /// <returns>A <see cref="DateTime"/> with the applied TimeZone.</returns>
        public static DateTime FromTimeZone(this DateTime dateTime, string timeZone) => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, timeZone);
    }
}
