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
/// This is a float type column.
/// </summary>
public sealed class NumberColumn : Column
{
    /// <summary>
    /// A number value that specifies the representing an exclusive upper limit for a value.
    /// </summary>
    [JsonPropertyName(PropertyNames.ExclusiveMaxValue)]
    [JsonPropertyOrder(13)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ExclusiveMaxValue { get; set; }

    /// <summary>
    /// A number value that specifies the representing an exclusive lower limit for a value.
    /// </summary>
    [JsonPropertyName(PropertyNames.ExclusiveMinValue)]
    [JsonPropertyOrder(11)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? ExclusiveMinValue { get; set; }

    /// <summary>
    /// A number value that specifies the minimum allowed value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MaxValue)]
    [JsonPropertyOrder(12)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? MaxValue { get; set; }

    /// <summary>
    /// A number value that specifies the maximum allowed value.
    /// </summary>
    [JsonPropertyName(PropertyNames.MinValue)]
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? MinValue { get; set; }
}