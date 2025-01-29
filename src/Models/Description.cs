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
    /// Human-readable description
    /// </summary>
    public class Description
    {
        /// <summary>
        /// The description itself
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Format of the description.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt to specify the language of the comment.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="Description"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="Description"/> instance</returns>
        internal static Description Parse(JsonElement jsonElement)
        {
            var description = new Description();

            if (jsonElement.TryGetStringProperty(PropertyNames.Language, out var languageProperty))
            {
                description.Language = languageProperty.GetString();
            }
            if (jsonElement.GetRequiredStringProperty(PropertyNames.Format, out var formatProperty))
            {
                description.Format = formatProperty.GetString();
            }
            if (jsonElement.GetRequiredStringProperty(PropertyNames.Content, out var contentProperty))
            {
                description.Content = contentProperty.GetString();
            }

            return description;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteStringOrNothing(PropertyNames.Language, Language);
            jsonWriter.WriteString(PropertyNames.Format, Format);
            jsonWriter.WriteString(PropertyNames.Content, Content);
            jsonWriter.WriteEndObject();
        }
    }
}