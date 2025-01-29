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
    /// Publisher that is responsible for publication and/or maintenance of the document.
    /// </summary>
    public class Publisher
    {
        /// <summary>
        /// Identifier for the publisher.
        /// </summary>
        public Identifier Identifier { get; set; }

        /// <summary>
        /// Human-readable name for the publisher.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// Short name for the publisher.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// More information about the publisher.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="Publisher"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="Publisher"/> instance</returns>
        internal static Publisher Parse(JsonElement jsonElement)
        {
            var publisher = new Publisher();

            if (jsonElement.GetRequiredStringProperty(PropertyNames.ShortName, out var shortNameProperty))
            {
                publisher.ShortName = shortNameProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.LongName, out var longNameProperty))
            {
                publisher.LongName = longNameProperty.GetString();
            }
            if (jsonElement.TryGetObjectProperty(PropertyNames.Identifier, out var identifierProperty))
            {
                publisher.Identifier = Identifier.Parse(identifierProperty);
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Url, out var urlProperty))
            {
                publisher.Url = new Uri(urlProperty.GetString());
            }

            return publisher;
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
            jsonWriter.WriteIdentifier(PropertyNames.Identifier, Identifier);
            jsonWriter.WriteUriOrNothing(PropertyNames.Url, Url);
            jsonWriter.WriteEndObject();
        }
    }
}