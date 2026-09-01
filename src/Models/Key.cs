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
/// A key definition.
/// </summary>
public sealed class Key : Owned<CodeListDocument>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Key"/> class.
    /// </summary>
    /// <param name="owner">The owner of the key</param>
    public Key(CodeListDocument owner) 
        : base(owner)
    {
        Columns = new ColumnRefs(owner);
    }

    /// <summary>
    /// A list of referenced columns
    /// </summary>
    public ColumnRefs Columns { get; }

    /// <summary>
    /// A brief description of the key
    /// </summary>
    public LocalizableString Description { get; set; }

    /// <summary>
    /// The unique ID of the key
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The name of the key 
    /// </summary>
    public LocalizableString Name { get; set; }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> object into a new <see cref="Key"/> instance.
    /// </summary>
    /// <param name="jsonElement">The json object</param>
    /// <returns>A new <see cref="Key"/> instance</returns>
    internal static Key Parse(CodeListDocument owner, JsonElement jsonElement)
    {
        var key = new Key(owner);

        if (jsonElement.GetRequiredStringProperty(PropertyNames.Id, out var idProperty))
        {
            key.Id = idProperty.GetString();
        }
        if (jsonElement.TryGetProperty(PropertyNames.Name, out var nameProperty))
        {
            key.Name = JsonSerializer.Deserialize<LocalizableString>(nameProperty, CodeListBase.JsonSerializerOptions);
        }
        if (jsonElement.TryGetProperty(PropertyNames.Description, out var descriptionProperty))
        {
            key.Description = JsonSerializer.Deserialize<LocalizableString>(descriptionProperty, CodeListBase.JsonSerializerOptions);
        }
        if (jsonElement.TryGetArrayProperty(PropertyNames.ColumnIds, out var columnIdsProperty))
        {
            key.Columns.ParseAndAdd(columnIdsProperty);
        }

        return key;
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
        jsonWriter.WriteEndObject();
    }
}