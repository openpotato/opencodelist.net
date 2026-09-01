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
/// An enumeration member.
/// </summary>
public sealed class EnumMember
{
    /// <summary>
    /// A short description of the value.
    /// </summary>
    [JsonPropertyName(PropertyNames.Description)]
    [JsonPropertyOrder(11)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LocalizableString Description { get; set; }

    /// <summary>
    /// The value.
    /// </summary>
    [JsonPropertyName(PropertyNames.Value)]
    [JsonPropertyOrder(10)]
    [JsonRequired]
    public string Value { get; set; }
}