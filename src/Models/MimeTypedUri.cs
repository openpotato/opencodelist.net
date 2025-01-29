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
    /// An inherited <see cref="Uri"/> class with an additional mime type declaration.
    /// </summary>
    public class MimeTypedUri : Uri
    {
        /// <summary>
        /// The mime type
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Uri"/> class.
        /// </summary>
        /// <param name="uriString">A string that identifies the resource to be represented by the <see cref="Uri"/> instance.</param>
        /// <param name="mimeType">A string that identifies the mime type of the resource.</param>
        public MimeTypedUri(string uriString, string mimeType)
            : base(uriString)
        {
            MimeType = mimeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Uri"/> class.
        /// </summary>
        /// <param name="uriString">A string that identifies the resource to be represented by the <see cref="Uri"/> instance.</param>
        /// <param name="mimeType">A string that identifies the mime type of the resource.</param>
        /// <param name="uriKind">Defines the kind of the <seealso cref="Uri"/>.</param>
        public MimeTypedUri(string uriString, string mimeType, UriKind uriKind)
            : base(uriString, uriKind)
        {
            MimeType = mimeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Uri"/> class.
        /// </summary>
        /// <param name="uriString">A string that identifies the resource to be represented by the <see cref="Uri"/> instance.</param>
        /// <param name="mimeType">A string that identifies the mime type of the resource.</param>
        /// <param name="creationOptions">Options that control how the <seealso cref="Uri"/> is created and behaves.</param>
        public MimeTypedUri(string uriString, string mimeType, in UriCreationOptions creationOptions)
            : base(uriString, creationOptions)
        {
            MimeType = mimeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Uri"/> class.
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="mimeType">A string that identifies the mime type of the resource.</param>
        /// <param name="relativeUri"></param>
        public MimeTypedUri(Uri baseUri, string mimeType, string relativeUri)
            : base(baseUri, relativeUri)
        {
            MimeType = mimeType;
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="MimeTypedUri"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="MimeTypedUri"/> instance</returns>
        internal static MimeTypedUri Parse(JsonElement jsonElement)
        {
            string mimeType = null;
            string uri = null;

            if (jsonElement.GetRequiredStringProperty(PropertyNames.MimeType, out var mimeTypeProperty))
            {
                mimeType = mimeTypeProperty.GetString();
            }
            if (jsonElement.GetRequiredStringProperty(PropertyNames.Url, out var uriProperty))
            {
                uri = uriProperty.GetString();
            }

            return new MimeTypedUri(uri, mimeType);
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.MimeType, MimeType);
            jsonWriter.WriteUriOrNothing(PropertyNames.Url, this);
            jsonWriter.WriteEndObject();
        }
    }
}