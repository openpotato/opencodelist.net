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

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenCodeList;

/// <summary>
/// Custom user annotation information.
/// </summary>
public sealed class Annotation
{
    /// <summary>
    /// Machine-readable information.
    /// </summary>
    [JsonPropertyName(PropertyNames.AppInfo)]
    [JsonPropertyOrder(1)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject AppInfo { get; set; }

    /// <summary>
    /// Human-readable descriptions.
    /// </summary>
    [JsonPropertyName(PropertyNames.Descriptions)]
    [JsonPropertyOrder(2)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Description> Descriptions { get; set; }
}