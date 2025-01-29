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

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OpenCodeList
{
    /// <summary>
    /// Custom user annotation information.
    /// </summary>
    public class Annotation
    {
        /// <summary>
        /// Machine-readable information.
        /// </summary>
        public JsonObject AppInfo { get; set; }

        /// <summary>
        /// Human-readable descriptions.
        /// </summary>
        public IList<Description> Descriptions { get; } = [];

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="Annotation"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="Annotation"/> instance</returns>
        internal static Annotation Parse(JsonElement jsonElement)
        {
            var annotation = new Annotation();

            if (jsonElement.GetRequiredArrayProperty(PropertyNames.Descriptions, out var descriptionsProperty))
            {
                foreach (var descriptionElement in descriptionsProperty.EnumerateArray())
                {
                    annotation.Descriptions.Add(Description.Parse(descriptionElement));
                }
            }
            if (jsonElement.TryGetObjectProperty(PropertyNames.AppInfo, out var appInfoProperty))
            {
                annotation.AppInfo = JsonObject.Create(appInfoProperty);
            }

            return annotation;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteDescriptionArray(PropertyNames.Descriptions, Descriptions);
            jsonWriter.WriteJsonObject(PropertyNames.AppInfo, AppInfo);
            jsonWriter.WriteEndObject();
        }
    }
}