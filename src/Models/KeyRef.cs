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
    /// Reference to a key in an external code list.
    /// </summary>
    public class KeyRef
    {
        /// <summary>
        /// Reference to an external code list.
        /// </summary>
        public CodeListDocumentRef CodeListRef { get; set; }

        /// <summary>
        /// Reference to a key ID in the external code list
        /// </summary>
        public string KeyId { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="KeyRef"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="KeyRef"/> instance</returns>
        internal static KeyRef Parse(JsonElement jsonElement)
        {
            var reference = new KeyRef();

            if (jsonElement.GetRequiredObjectProperty(PropertyNames.CodeListRef, out var codeListRefProperty))
            {
                reference.CodeListRef = CodeListDocumentRef.Parse(codeListRefProperty);
            }
            if (jsonElement.GetRequiredStringProperty(PropertyNames.KeyId, out var keyIdProperty))
            {
                reference.KeyId = keyIdProperty.GetString();
            }

            return reference;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteCodeListRef(PropertyNames.CodeListRef, CodeListRef);
            jsonWriter.WriteString(PropertyNames.KeyId, KeyId);
            jsonWriter.WriteEndObject();
        }
    }
}