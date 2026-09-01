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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenCodeList;

/// <summary>
/// This is a string type column.
/// </summary>
public sealed class StringColumn: Column
{
    /// <summary>
    /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt to specify the language of the
    /// content.
    /// </summary>
    [JsonPropertyName(PropertyNames.Language)]
    [JsonPropertyOrder(14)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Language { get; set; }

    /// <summary>
    /// An integer that specifies the maximum character length of the value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MaxLength)]
    [JsonPropertyOrder(11)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxLength { get; set; }

    /// <summary>
    /// An integer that specifies the minimum character length of the value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MinLength)]
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinLength { get; set; }

    /// <summary>
    /// A string that specifies a regular expression that must mach against each value.
    /// </summary>
    [JsonPropertyName(PropertyNames.Pattern)]
    [JsonPropertyOrder(13)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Pattern { get; set; }
}