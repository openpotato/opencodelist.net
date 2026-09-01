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
/// This is a time only type column. The serialized format must match the format <c>HH:mm:ss[.fffffff]</c>.
/// </summary>
public sealed class TimeOnlyColumn : Column
{
    /// <summary>
    /// A value that specifies the maximum allowed value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MaxValue)]
    [JsonPropertyOrder(11)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TimeOnly? MaxValue { get; set; }

    /// <summary>
    /// A value that specifies the minimum allowed value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MinValue)]
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TimeOnly? MinValue { get; set; }
}