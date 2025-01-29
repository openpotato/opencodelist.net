#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    OpenCodeList.NET 
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 * 
 */
#endregion

using System.Text.Json;

namespace OpenCodeList
{
    /// <summary>
    /// A key definition.
    /// </summary>
    public class Key
    {
        private readonly CodeListDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        /// <param name="owner">The owner of the key</param>
        public Key(CodeListDocument document)
        {
            _document = document;
            Columns = new Columns(_document);
        }

        /// <summary>
        /// A list of referenced columns
        /// </summary>
        public Columns Columns { get; }

        /// <summary>
        /// A brief description of the key
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The unique ID of the key
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the key 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="Key"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="Key"/> instance</returns>
        internal static Key Parse(JsonElement jsonElement, CodeListDocument codeList)
        {
            var key = new Key(codeList);

            if (jsonElement.GetRequiredStringProperty(PropertyNames.Id, out var idProperty))
            {
                key.Id = idProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Name, out var nameProperty))
            {
                key.Name = nameProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Description, out var descriptionProperty))
            {
                key.Description = descriptionProperty.GetString();
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
            jsonWriter.WriteColumnIdArray(PropertyNames.ColumnIds, Columns);
            jsonWriter.WriteEndObject();
        }
    }
}