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
    /// A general identifier
    /// </summary>
    public class Identifier
    {
        /// <summary>
        /// The identifier value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The source of the identifier.
        /// </summary>
        public IdentifierSource Source { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="Identifier"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="Identifier"/> instance</returns>
        internal static Identifier Parse(JsonElement jsonElement)
        {
            var identifier = new Identifier();

            if (jsonElement.GetRequiredStringProperty(PropertyNames.Value, out var valueProperty))
            {
                identifier.Value = valueProperty.GetString();
            }
            if (jsonElement.TryGetObjectProperty(PropertyNames.Source, out var sourceProperty))
            {
                identifier.Source = IdentifierSource.Parse(sourceProperty);
            }

            return identifier;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.Value, Value);
            jsonWriter.WriteSource(PropertyNames.Source, Source);
            jsonWriter.WriteEndObject();
        }
    }
}