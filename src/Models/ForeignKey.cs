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
    /// A foreign key definition
    /// </summary>
    public class ForeignKey
    {
        private readonly CodeListDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKey"/> class.
        /// </summary>
        /// <param name="document">The owner of the foreign key</param>
        public ForeignKey(CodeListDocument document)
        {
            _document = document;
            Columns = new Columns(_document);
        }

        /// <summary>
        /// A list of column IDs in the current code list.
        /// </summary>
        public Columns Columns { get; }

        /// <summary>
        /// A short description of the foreign key.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The ID of the foreign key.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the foreign key.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A list of column IDs in the current code list.
        /// </summary>
        public KeyRef KeyRef { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="ForeignKey"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="ForeignKey"/> instance</returns>
        internal static ForeignKey Parse(JsonElement jsonElement, CodeListDocument codeList)
        {
            var foreignKey = new ForeignKey(codeList);

            if (jsonElement.TryGetProperty(PropertyNames.Id, out var idProperty))
            {
                foreignKey.Id = idProperty.GetString();
            }
            if (jsonElement.TryGetProperty(PropertyNames.Name, out var nameProperty))
            {
                foreignKey.Name = nameProperty.GetString();
            }
            if (jsonElement.TryGetProperty(PropertyNames.Description, out var descriptionProperty))
            {
                foreignKey.Description = descriptionProperty.GetString();
            }
            if (jsonElement.TryGetArrayProperty(PropertyNames.ColumnIds, out var columnIdsProperty))
            {
                foreignKey.Columns.ParseAndAdd(columnIdsProperty);
            }
            if (jsonElement.TryGetObjectProperty(PropertyNames.KeyRef, out var keyRefProperty))
            {
                foreignKey.KeyRef = KeyRef.Parse(keyRefProperty);
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
            jsonWriter.WriteColumnIdArray(PropertyNames.ColumnIds, Columns);
            jsonWriter.WriteReference(PropertyNames.KeyRef, KeyRef);
            jsonWriter.WriteEndObject();
        }
    }
}