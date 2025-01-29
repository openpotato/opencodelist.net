#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    OpenCodeList.NET 
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 * 
 */
#endregion

using System;
using System.Globalization;

namespace OpenCodeList
{
    /// <summary>
    /// Utils for parsing date-time values
    /// </summary>
    public static class DateTimeUtils
    {
        public static readonly string[] DateOnlyFormats =
        {
            "yyyy-MM-dd"
        };

        public static readonly string[] DateTimeFormats =
                { 
            "yyyy-MM-dd'T'HH:mm:ss.FFFK",
            "yyyy-MM-dd'T'HH:mm.FFFK",
            "yyyy-MM-dd"
        };
        
        public static readonly string[] TimeOnlyFormats =
        {
            "HH:mm:ss",
            "HH:mm"
        };

        public static DateOnly ParseDateOnly(string value)
        {
            return DateOnly.ParseExact(value, DateOnlyFormats, CultureInfo.InvariantCulture);
        }

        public static DateTimeOffset ParseDateTimeOffset(string value)
        {
            return DateTimeOffset.ParseExact(value, DateTimeFormats, CultureInfo.InvariantCulture);
        }

        public static TimeOnly ParseTimeOnly(string value)
        {
            return TimeOnly.ParseExact(value, TimeOnlyFormats, CultureInfo.InvariantCulture);
        }
    }
}
