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
/// Reference to a key in an external code list.
/// </summary>
public sealed class ExternalKeyRef
{
    /// <summary>
    /// Reference to an external code list.
    /// </summary>
    [JsonPropertyName(PropertyNames.CodeListRef)]
    [JsonPropertyOrder(11)]
    [JsonRequired]
    public ExternalCodeListDocumentRef CodeListRef { get; set; }

    /// <summary>
    /// Reference to a key ID in the external code list
    /// </summary>
    [JsonPropertyName(PropertyNames.KeyId)]
    [JsonPropertyOrder(11)]
    [JsonRequired]
    public string KeyId { get; set; }
}