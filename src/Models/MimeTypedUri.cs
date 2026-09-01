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
/// An inherited <see cref="Uri"/> class with an additional mime type declaration.
/// </summary>
public sealed class MimeTypedUri
{
    /// <summary>
    /// The mime type
    /// </summary>
    [JsonPropertyName(PropertyNames.MimeType)]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string MimeType { get; set; }

    /// <summary>
    /// The location for retrieving the referenced resource
    /// </summary>
    [JsonPropertyName(PropertyNames.Url)]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public Uri Url { get; set; }
}