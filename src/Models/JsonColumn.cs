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

using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OpenCodeList
{
    /// <summary>
    /// This is a column representing an embedded json object or array.
    /// </summary>
    public class JsonColumn : Column
    {
        /// <summary>
        /// Emebdded JSON schema as string.
        /// </summary>
        public JsonObject EmbeddedSchema { get; set; }

        /// <summary>
        /// Uri to the JSON schema file.
        /// </summary>
        public Uri ExternalSchema { get; set; }

        /// <summary>
        /// Schema location
        /// </summary>
        public JsonColumnSchemaLocation SchemaLocation { get; set; } = JsonColumnSchemaLocation.External;

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="JsonColumn"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="JsonColumn"/> instance</returns>
        internal static JsonColumn Parse(JsonElement jsonElement)
        {
            var column = new JsonColumn();

            if (jsonElement.GetRequiredStringProperty(PropertyNames.Id, out var idProperty))
            {
                column.Id = idProperty.GetString();
            }
            if (jsonElement.GetRequiredStringProperty(PropertyNames.Name, out var nameProperty))
            {
                column.Name = nameProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Description, out var descriptionProperty))
            {
                column.Description = descriptionProperty.GetString();
            }
            if (jsonElement.TryGetBooleanProperty(PropertyNames.Nullable, out var nullableProperty))
            {
                column.Nullable = nullableProperty.GetBoolean();
            }
            if (jsonElement.TryGetBooleanProperty(PropertyNames.Optional, out var optionalProperty))
            {
                column.Optional = optionalProperty.GetBoolean();
            }
            if (jsonElement.TryGetProperty(PropertyNames.Schema, out var schemaProperty))
            {
                if (schemaProperty.ValueKind == JsonValueKind.String)
                {
                    column.SchemaLocation = JsonColumnSchemaLocation.External;
                    column.ExternalSchema = new Uri(schemaProperty.GetString());
                }
                else if (schemaProperty.ValueKind == JsonValueKind.Object)
                {
                    column.SchemaLocation = JsonColumnSchemaLocation.Embedded;
                    column.EmbeddedSchema = JsonObject.Create(schemaProperty);
                }
                else
                {
                    throw new CodeListParserException($"JSON type \"{schemaProperty.ValueKind}\" not allowed.");
                }
            }

            return column;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal override void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.Type, TypeConsts.Document);
            jsonWriter.WriteString(PropertyNames.Id, Id);
            jsonWriter.WriteString(PropertyNames.Name, Name);
            jsonWriter.WriteStringOrNothing(PropertyNames.Description, Description);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Nullable, Nullable);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Optional, Optional);
            if (SchemaLocation == JsonColumnSchemaLocation.External)
            {
                jsonWriter.WriteUriOrNothing(PropertyNames.Schema, ExternalSchema);
            }
            else if (EmbeddedSchema != null)
            {
                jsonWriter.WritePropertyName(PropertyNames.Schema);
                JsonSerializer.Serialize(jsonWriter, EmbeddedSchema);
            }
            jsonWriter.WriteEndObject();
        }
    }
}