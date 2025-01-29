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

namespace OpenCodeList
{
    /// <summary>
    /// Source information for a general identifier
    /// </summary>
    public class IdentifierSource
    {
        /// <summary>
        /// Human-readable name of the source.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// Short name of the source.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// More information about the source.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="IdentifierSource"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="IdentifierSource"/> instance</returns>
        internal static IdentifierSource Parse(JsonElement jsonElement)
        {
            var source = new IdentifierSource();

            if (jsonElement.GetRequiredStringProperty(PropertyNames.ShortName, out var shortNameProperty))
            {
                source.ShortName = shortNameProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.LongName, out var longNameProperty))
            {
                source.LongName = longNameProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Url, out var urlProperty))
            {
                source.Url = new Uri(urlProperty.GetString());
            }

            return source;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.ShortName, ShortName);
            jsonWriter.WriteStringOrNothing(PropertyNames.LongName, LongName);
            jsonWriter.WriteUriOrNothing(PropertyNames.Url, Url);
            jsonWriter.WriteEndObject();
        }
    }
}