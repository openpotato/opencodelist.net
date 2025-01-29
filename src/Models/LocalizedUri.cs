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
    /// An inherited <see cref="Uri"/> class with an additional language declaration.
    /// </summary>
    public class LocalizedUri : Uri
    {
        /// <summary>
        /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedUri"/> class.
        /// </summary>
        /// <param name="uriString">A string that identifies the resource to be represented by the <see cref="Uri"/> instance.</param>
        /// <param name="language">A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt..</param>
        public LocalizedUri(string uriString, string language)
            : base(uriString)
        {
            Language = language;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedUri"/> class.
        /// </summary>
        /// <param name="uriString">A string that identifies the resource to be represented by the <see cref="Uri"/> instance.</param>
        /// <param name="language">A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt..</param>
        /// <param name="uriKind">Defines the kind of the <seealso cref="Uri"/>.</param>
        public LocalizedUri(string uriString, string language, UriKind uriKind)
            : base(uriString, uriKind)
        {
            Language = language;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedUri"/> class.
        /// </summary>
        /// <param name="uriString">A string that identifies the resource to be represented by the <see cref="Uri"/> instance.</param>
        /// <param name="language">A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt..</param>
        /// <param name="creationOptions">Options that control how the <seealso cref="Uri"/> is created and behaves.</param>
        public LocalizedUri(string uriString, string language, in UriCreationOptions creationOptions)
            : base(uriString, creationOptions)
        {
            Language = language;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedUri"/> class.
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="language">A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt..</param>
        /// <param name="relativeUri"></param>
        public LocalizedUri(Uri baseUri, string language, string relativeUri)
            : base(baseUri, relativeUri)
        {
            Language = language;
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="LocalizedUri"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="MimeTypedUri"/> instance</returns>
        internal static LocalizedUri Parse(JsonElement jsonElement)
        {
            string language = null;
            string uri = null;

            if (jsonElement.GetRequiredStringProperty(PropertyNames.Language, out var languageProperty))
            {
                language = languageProperty.GetString();
            }
            if (jsonElement.GetRequiredStringProperty(PropertyNames.Url, out var uriProperty))
            {
                uri = uriProperty.GetString();
            }

            return new LocalizedUri(uri, language);
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.Language, Language);
            jsonWriter.WriteUriOrNothing(PropertyNames.Url, this);
            jsonWriter.WriteEndObject();
        }
    }
}