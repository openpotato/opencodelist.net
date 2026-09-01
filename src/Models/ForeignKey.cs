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

using System.Text.Json;

namespace OpenCodeList;

/// <summary>
/// A foreign key definition
/// </summary>
public sealed class ForeignKey : Owned<CodeListDocument>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKey"/> class.
    /// </summary>
    /// <param name="owner">The owner of the foreign key</param>
    public ForeignKey(CodeListDocument owner)
        : base(owner)
    {
        Columns = new ColumnRefs(owner);
    }

    /// <summary>
    /// A list of column IDs in the current code list.
    /// </summary>
    public ColumnRefs Columns { get; }

    /// <summary>
    /// A short description of the foreign key.
    /// </summary>
    public LocalizableString Description { get; set; }

    /// <summary>
    /// The ID of the foreign key.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// A list of column IDs in the current code list.
    /// </summary>
    public ExternalKeyRef KeyRef { get; set; }

    /// <summary>
    /// The name of the foreign key.
    /// </summary>
    public LocalizableString Name { get; set; }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> object into a new <see cref="ForeignKey"/> instance.
    /// </summary>
    /// <param name="jsonElement">The json object</param>
    /// <returns>A new <see cref="ForeignKey"/> instance</returns>
    internal static ForeignKey Parse(CodeListDocument owner, JsonElement jsonElement)
    {
        var foreignKey = new ForeignKey(owner);

        if (jsonElement.TryGetProperty(PropertyNames.Id, out var idProperty))
        {
            foreignKey.Id = idProperty.GetString();
        }
        if (jsonElement.TryGetProperty(PropertyNames.Name, out var nameProperty))
        {
            foreignKey.Name = JsonSerializer.Deserialize<LocalizableString>(nameProperty, CodeListBase.JsonSerializerOptions);
        }
        if (jsonElement.TryGetProperty(PropertyNames.Description, out var descriptionProperty))
        {
            foreignKey.Description = JsonSerializer.Deserialize<LocalizableString>(descriptionProperty, CodeListBase.JsonSerializerOptions);
        }
        if (jsonElement.TryGetArrayProperty(PropertyNames.ColumnIds, out var columnIdsProperty))
        {
            foreignKey.Columns.ParseAndAdd(columnIdsProperty);
        }
        if (jsonElement.TryGetObjectProperty(PropertyNames.KeyRef, out var keyRefProperty))
        {
            foreignKey.KeyRef = JsonSerializer.Deserialize<ExternalKeyRef>(keyRefProperty, CodeListBase.JsonSerializerOptions);
        }

        return foreignKey;
    }

    /// <summary>
    /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
    /// </summary>
    /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
    internal void WriteTo(Utf8JsonWriter jsonWriter)
    {
        jsonWriter.WriteStartObject();
        jsonWriter.WriteString(PropertyNames.Id, Id);
        jsonWriter.WriteStringOrNothing(PropertyNames.Name, Name);
        jsonWriter.WriteStringOrNothing(PropertyNames.Description, Description);
        jsonWriter.WriteColumnRefsArrayOrNothing(PropertyNames.ColumnIds, Columns);
        jsonWriter.WriteReference(PropertyNames.KeyRef, KeyRef);
        jsonWriter.WriteEndObject();
    }
}