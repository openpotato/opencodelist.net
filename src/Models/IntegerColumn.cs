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
/// This is a integer type column.
/// </summary>
public sealed class IntegerColumn : Column
{
    /// <summary>
    /// An integer value that specifies the maximum allowed value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MaxValue)]
    [JsonPropertyOrder(11)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? MaxValue { get; set; }

    /// <summary>
    /// An integer value that specifies the minimum allowed value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MinValue)]
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? MinValue { get; set; }
}