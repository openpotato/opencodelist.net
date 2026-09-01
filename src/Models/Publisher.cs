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
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenCodeList;

/// <summary>
/// Publisher that is responsible for publication and/or maintenance of the document.
/// </summary>
public sealed class Publisher
{
    /// <summary>
    /// A dictionary to hold any additional properties that are not explicitly defined in the class. 
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement> Extensions { get; set; }

    /// <summary>
    /// Identifier for the publisher.
    /// </summary>
    [JsonPropertyName(PropertyNames.Identifier)]
    [JsonPropertyOrder(3)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Identifier Identifier { get; set; }

    /// <summary>
    /// Human-readable name for the publisher.
    /// </summary>
    [JsonPropertyName(PropertyNames.LongName)]
    [JsonPropertyOrder(2)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string LongName { get; set; }

    /// <summary>
    /// Short name for the publisher.
    /// </summary>
    [JsonPropertyName(PropertyNames.ShortName)]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string ShortName { get; set; }

    /// <summary>
    /// More information about the publisher.
    /// </summary>
    [JsonPropertyName(PropertyNames.Url)]
    [JsonPropertyOrder(4)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri Url { get; set; }
}