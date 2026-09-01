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
/// Source information for a general identifier
/// </summary>
public sealed class IdentifierSource
{
    /// <summary>
    /// Human-readable name of the source.
    /// </summary>
    [JsonPropertyName(PropertyNames.LongName)]
    [JsonPropertyOrder(2)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string LongName { get; set; }

    /// <summary>
    /// Short name of the source.
    /// </summary>
    [JsonPropertyName(PropertyNames.ShortName)]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string ShortName { get; set; }

    /// <summary>
    /// More information about the source.
    /// </summary>
    [JsonPropertyName(PropertyNames.Url)]
    [JsonPropertyOrder(3)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri Url { get; set; }
}