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

using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenCodeList
{
    /// <summary>
    /// A generic OpenCodeList document loader
    /// </summary>
    public static class DocumentLoader
    {
        /// <summary>
        /// Loads a new document from a stream. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The input stream</param>
        /// <returns>Either a new <see cref="CodeListDocument"/> or a new <see cref="CodeListSetDocument"/> instance</returns>
        public static Document Load(Stream stream)
        {
            var jsonDocument = JsonDocument.Parse(stream, default);

            return Parse(jsonDocument.RootElement);
        }

        /// <summary>
        /// Loads a new document from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file info</param>
        /// <returns>Either a new <see cref="CodeListDocument"/> or a new <see cref="CodeListSetDocument"/> instance</returns>
        public static Document Load(FileInfo fileInfo)
        {
            return Load(fileInfo.FullName);
        }

        /// <summary>
        /// Loads a new document from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns>Either a new <see cref="CodeListDocument"/> or a new <see cref="CodeListSetDocument"/> instance</returns>
        public static Document Load(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);

            return Load(fileStream);
        }

        /// <summary>
        /// Loads a new document from a stream. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The input stream</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous load operation. Returns either a new <see cref="CodeListDocument"/> or 
        /// a new <see cref="CodeListSetDocument"/> instance</returns>
        public static async Task<Document> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var jsonDocument = await JsonDocument.ParseAsync(stream, default, cancellationToken);

            return Parse(jsonDocument.RootElement);
        }

        /// <summary>
        /// Loads a new document from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file info</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous load operation. Returns either a new <see cref="CodeListDocument"/> or 
        /// a new <see cref="CodeListSetDocument"/> instance</returns>
        public static Task<Document> LoadAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            return LoadAsync(fileInfo.FullName, cancellationToken);
        }

        /// <summary>
        /// Loads a new document from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous load operation. Returns either a new <see cref="CodeListDocument"/> or 
        /// a new <see cref="CodeListSetDocument"/> instance</returns>
        public static async Task<Document> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            using var fileStream = File.OpenRead(filePath);

            return await LoadAsync(fileStream, cancellationToken);
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a document.
        /// </summary>
        /// <param name="rootElement">The JSON object</param>
        /// <returns>Either a new <see cref="CodeListDocument"/> or a new <see cref="CodeListSetDocument"/> instance</returns>
        /// <exception cref="CodeListParserException">Syntax error</exception>
        private static Document Parse(JsonElement rootElement)
        {
            if (rootElement.ValueKind == JsonValueKind.Object)
            {
                if (rootElement.TryGetProperty(PropertyNames.OpenCodeList, out var versionProperty))
                {
                    if (SemanticVersion.From(versionProperty.GetString()) >= Document.GetVersion())
                    {
                        if (rootElement.TryGetProperty(PropertyNames.CodeList, out var codeListProperty))
                        {
                            return CodeListDocument.ParseContent(rootElement, codeListProperty);
                        }
                        else if (rootElement.TryGetProperty(PropertyNames.CodeListSet, out var codeListSetProperty))
                        {
                            return CodeListSetDocument.ParseContent(rootElement, codeListSetProperty);
                        }
                        else
                        {
                            throw new CodeListParserException($"JSON Property \"{PropertyNames.CodeList}\" or \"{PropertyNames.CodeListSet}\" missing.");
                        }
                    }
                    else
                    {
                        throw new CodeListParserException($"Version {versionProperty.GetString()} of OpenCodeList not supported.");
                    }
                }
                else
                {
                    throw new CodeListParserException($"JSON Property \"{PropertyNames.OpenCodeList}\" missing.");
                }
            }
            else
            {
                throw new CodeListParserException($"JSON Object expected.");
            }
        }
    }
}