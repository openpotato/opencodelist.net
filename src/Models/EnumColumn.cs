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
using System.Text.Json.Serialization;

namespace OpenCodeList;

/// <summary>
/// This is an enumeration type column
/// </summary>
public sealed class EnumColumn : Column
{
    /// <summary>
    /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt to specify the language of the 
    /// content.
    /// </summary>
    [JsonPropertyName(PropertyNames.Language)]
    [JsonPropertyOrder(11)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Language { get; set; }

    /// <summary>
    /// The list of allowed values for this enumeration.
    /// </summary>
    [JsonPropertyName(PropertyNames.Members)]
    [JsonPropertyOrder(10)]
    [JsonRequired]
    public IList<EnumMember> Members { get; set;  } = [];
}