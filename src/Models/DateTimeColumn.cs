#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    OpenCodeList.NET 
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License. 
 * 
 */
#endregion

using System;
using System.Text.Json.Serialization;

namespace OpenCodeList;

/// <summary>
/// This is a date-time type column. The serialized format must match the native JSON string with the JSON Schema 
/// format `date-time`. See: https://json-schema.org/understanding-json-schema/reference/string.
/// </summary>
public sealed class DateTimeColumn : Column
{
    /// <summary>
    /// A value that specifies the maximum allowed value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MaxValue)]
    [JsonPropertyOrder(11)] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? MaxValue { get; set; }

    /// <summary>
    /// An value that specifies the minimum allowed value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MinValue)]
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? MinValue { get; set; }
}