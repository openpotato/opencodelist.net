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
    /// This is a string type column.
    /// </summary>
    public class StringColumn: Column
    {
        /// <summary>
        /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt to specify the language of the content.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// An integer that specifies the maximum character length of the value.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// An integer that specifies the minimum character length of the value.
        /// </summary>
        public int? MinLength { get; set; }
        /// <summary>
        /// A string that specifies a regular expression that must mach against each value.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="StringColumn"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="StringColumn"/> instance</returns>
        internal static StringColumn Parse(JsonElement jsonElement)
        {
            var column = new StringColumn();

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
            if (jsonElement.TryGetProperty(PropertyNames.MinLength, out var minLengthProperty))
            {
                column.MinLength = minLengthProperty.GetInt32();
            }
            if (jsonElement.TryGetProperty(PropertyNames.MaxLength, out var maxLengthProperty))
            {
                column.MaxLength = maxLengthProperty.GetInt32();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Pattern, out var patternProperty))
            {
                column.Pattern = patternProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Language, out var languageProperty))
            {
                column.Language = languageProperty.GetString();
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
            jsonWriter.WriteString(PropertyNames.Type, TypeConsts.String);
            jsonWriter.WriteString(PropertyNames.Id, Id);
            jsonWriter.WriteString(PropertyNames.Name, Name);
            jsonWriter.WriteStringOrNothing(PropertyNames.Description, Description);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Nullable, Nullable);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Optional, Optional);
            jsonWriter.WriteIntegerOrNothing(PropertyNames.MinLength, MinLength);
            jsonWriter.WriteIntegerOrNothing(PropertyNames.MaxLength, MaxLength);
            jsonWriter.WriteStringOrNothing(PropertyNames.Pattern, Pattern);
            jsonWriter.WriteStringOrNothing(PropertyNames.Language, Language);
            jsonWriter.WriteEndObject();
        }
    }
}