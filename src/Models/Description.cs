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

using System.Text.Json.Serialization;

namespace OpenCodeList;

/// <summary>
/// Human-readable description
/// </summary>
public sealed class Description
{
    /// <summary>
    /// The description itself
    /// </summary>
    [JsonPropertyName(PropertyNames.Content)]
    [JsonPropertyOrder(3)]
    [JsonRequired]
    public string Content { get; set; }

    /// <summary>
    /// Format of the description.
    /// </summary>
    [JsonPropertyName(PropertyNames.Format)]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public string Format { get; set; }

    /// <summary>
    /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt to specify the 
    /// language of the comment.
    /// </summary>
    [JsonPropertyName(PropertyNames.Language)]
    [JsonPropertyOrder(1)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Language { get; set; }
}