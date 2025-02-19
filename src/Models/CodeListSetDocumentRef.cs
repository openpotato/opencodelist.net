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
    /// An external code list reference.
    /// </summary>
    public class CodeListSetDocumentRef : DocumentRef
    {
        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="CodeListDocumentRef"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="CodeListSetDocumentRef"/> instance</returns>
        internal static CodeListSetDocumentRef Parse(JsonElement jsonElement)
        {
            var documentRef = new CodeListSetDocumentRef();

            if (jsonElement.GetRequiredStringProperty(PropertyNames.CanonicalUri, out var canonicalUriProperty))
            {
                documentRef.CanonicalUri = new Uri(canonicalUriProperty.GetString());
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.CanonicalVersionUri, out var canonicalVersionUriProperty))
            {
                documentRef.CanonicalVersionUri = new Uri(canonicalVersionUriProperty.GetString());
            }
            if (jsonElement.TryGetArrayProperty(PropertyNames.LocationUrls, out var locationUrisProperty))
            {
                foreach (var locationUriElement in locationUrisProperty.EnumerateArray())
                {
                    documentRef.LocationUrls.Add(new Uri(locationUriElement.GetString()));
                }
            }

            return documentRef;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal override void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.Type, TypeConsts.CodeListSetRef);
            jsonWriter.WriteUri(PropertyNames.CanonicalUri, CanonicalUri);
            jsonWriter.WriteUriOrNothing(PropertyNames.CanonicalVersionUri, CanonicalVersionUri);
            jsonWriter.WriteUriArray(PropertyNames.LocationUrls, LocationUrls);
            jsonWriter.WriteEndObject();
        }
    }
}
