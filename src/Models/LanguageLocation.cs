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
/// An inherited <see cref="Uri"/> class with an additional language declaration.
/// </summary>
public sealed class LanguageLocation
{
    /// <summary>
    /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt.
    /// </summary>
    [JsonPropertyName(PropertyNames.Language)]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string Language { get; set; }

    /// <summary>
    /// The location for retrieving the referenced resource
    /// </summary>
    [JsonPropertyName(PropertyNames.Url)]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public Uri Url { get; set; }
}