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
/// This is a code list column.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = PropertyNames.Type)]
[JsonDerivedType(typeof(StringColumn), TypeConsts.String)]
[JsonDerivedType(typeof(EnumColumn), TypeConsts.Enum)]
[JsonDerivedType(typeof(EnumSetColumn), TypeConsts.EnumSet)]
[JsonDerivedType(typeof(NumberColumn), TypeConsts.Number)]
[JsonDerivedType(typeof(IntegerColumn), TypeConsts.Integer)]
[JsonDerivedType(typeof(BooleanColumn), TypeConsts.Boolean)]
[JsonDerivedType(typeof(DateOnlyColumn), TypeConsts.DateOnly)]
[JsonDerivedType(typeof(DateTimeColumn), TypeConsts.DateTime)]
[JsonDerivedType(typeof(TimeOnlyColumn), TypeConsts.TimeOnly)]
[JsonDerivedType(typeof(JsonColumn), TypeConsts.Document)]
public abstract class Column
{
    /// <summary>
    /// A human-readable description of the code list column.
    /// </summary>
    [JsonPropertyName(PropertyNames.Description)]
    [JsonPropertyOrder(3)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LocalizableString Description { get; set; }

    /// <summary>
    /// The ID of the code list column.
    /// </summary>
    [JsonPropertyName(PropertyNames.Id)]
    [JsonPropertyOrder(1)]
    [JsonRequired]
    public string Id { get; set; }

    /// <summary>
    /// The name of the code list column.
    /// </summary>
    [JsonPropertyName(PropertyNames.Name)]
    [JsonPropertyOrder(2)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LocalizableString Name { get; set; }

    /// <summary>
    /// A boolean that specifies whether thecolumn value can be `null`.
    /// </summary>
    [JsonPropertyName(PropertyNames.Nullable)]
    [JsonPropertyOrder(4)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Nullable { get; set; }

    /// <summary>
    /// A boolean that defines whether this column is optional, i.e. whether it can be completely 
    /// omitted from a data row.
    /// </summary>
    [JsonPropertyName(PropertyNames.Optional)]
    [JsonPropertyOrder(5)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Optional { get; set; }
}