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
/// This is a column representing an embedded json object or array.
/// </summary>
public sealed class JsonColumn : Column
{
    /// <summary>
    /// Uri to the optional JSON schema file.
    /// </summary>
    [JsonPropertyName(PropertyNames.SchemaUri)]
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri SchemaUri { get; set; }
}