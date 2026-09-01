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
/// A general identifier
/// </summary>
public sealed class Identifier
{
    /// <summary>
    /// The source of the identifier.
    /// </summary>
    [JsonPropertyName(PropertyNames.Source)]
    [JsonPropertyOrder(2)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IdentifierSource Source { get; set; }

    /// <summary>
    /// The identifier value.
    /// </summary>
    [JsonPropertyName(PropertyNames.Value)]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string Value { get; set; }
}