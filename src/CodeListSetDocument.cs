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
using System.Threading.Tasks;
using System.Threading;

namespace OpenCodeList
{
    /// <summary>
    /// A code list set document according to the OpenCodeList specification
    /// </summary>
    public class CodeListSetDocument : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeListSetDocument"/> class.
        /// </summary>
        public CodeListSetDocument()
            : base()
        {
            DocumentRefs = new DocumentRefs(this);
        }

        /// <summary>
        /// The list of document references.
        /// </summary>
        public DocumentRefs DocumentRefs { get; }

        /// <summary>
        /// Loads a new code list set from a stream. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The input stream</param>
        public static CodeListSetDocument Load(Stream stream)
        {
            var jsonDocument = JsonDocument.Parse(stream, default);

            return Parse(jsonDocument.RootElement);
        }

        /// <summary>
        /// Loads a new code list set from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file info</param>
        public static CodeListSetDocument Load(FileInfo fileInfo)
        {
            return Load(fileInfo.FullName);
        }

        /// <summary>
        /// Loads a new code list set from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        public static CodeListSetDocument Load(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);

            return Load(fileStream);
        }

        /// <summary>
        /// Loads a new code list set from a stream. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The input stream</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous load operation.</returns>
        public static async Task<CodeListSetDocument> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var jsonDocument = await JsonDocument.ParseAsync(stream, default, cancellationToken);

            return Parse(jsonDocument.RootElement);
        }

        /// <summary>
        /// Loads a new code list set from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file info</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous load operation.</returns>
        public static Task<CodeListSetDocument> LoadAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            return LoadAsync(fileInfo.FullName, cancellationToken);
        }

        /// <summary>
        /// Loads a new code list set from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous load operation.</returns>
        public static async Task<CodeListSetDocument> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            using var fileStream = File.OpenRead(filePath);

            return await LoadAsync(fileStream, cancellationToken);
        }

        /// <summary>
        /// Clears only the content of this document instance
        /// </summary>
        /// <param name="convertToMetaOnly">If TRUE, marks document as meta document</param>
        public override void ClearContent(bool convertToMetaOnly)
        {
            DocumentRefs.Clear();
            base.ClearContent(convertToMetaOnly);
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object (representing the complete document) into a code list set document
        /// </summary>
        /// <param name="rootElement">The JSON document root object</param>
        /// <returns>A new <see cref="CodeListSetDocument"/> instance</returns>
        /// <exception cref="CodeListParserException">Syntax error</exception>
        internal static CodeListSetDocument Parse(JsonElement rootElement)
        {
            if (rootElement.ValueKind == JsonValueKind.Object)
            {
                if (rootElement.TryGetProperty(PropertyNames.OpenCodeList, out var versionProperty))
                {
                    if (SemanticVersion.From(versionProperty.GetString()) < GetVersion())
                    {
                        throw new CodeListParserException($"Version {versionProperty.GetString()} of OpenCodeList not supported.");
                    }
                }
                else
                {
                    throw new CodeListParserException($"JSON Property \"{PropertyNames.OpenCodeList}\" missing.");
                }

                if (rootElement.GetRequiredObjectProperty(PropertyNames.CodeListSet, out var codeListSetProperty))
                {
                    return ParseContent(rootElement, codeListSetProperty);
                }
                else
                {
                    throw new CodeListParserException($"JSON Property \"{PropertyNames.CodeListSet}\" missing.");
                }
            }
            else
            {
                throw new CodeListParserException($"JSON Object expected.");
            }
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object (representing the inner part of the document) into a code list set document
        /// </summary>
        /// <param name="rootElement">The JSON document root object</param>
        /// <param name="codeListSetElement">The JSON sub object for the code list set</param>
        /// <returns>A new <see cref="CodeListSetDocument"/> instance</returns>
        internal static CodeListSetDocument ParseContent(JsonElement rootElement, JsonElement codeListSetElement)
        {
            var document = new CodeListSetDocument();

            if (rootElement.TryGetArrayProperty(PropertyNames.Comments, out var commentsProperty))
            {
                foreach (var commentElement in commentsProperty.EnumerateArray())
                {
                    document.Comments.Add(commentElement.GetString());
                }
            }
            if (codeListSetElement.TryGetObjectProperty(PropertyNames.Annotation, out var annotationProperty))
            {
                document.Annotation = Annotation.Parse(annotationProperty);
            }
            if (codeListSetElement.GetRequiredObjectProperty(PropertyNames.Identification, out var identificationProperty))
            {
                document.Identification = Identification.Parse(identificationProperty);
            }
            if (codeListSetElement.TryGetArrayProperty(PropertyNames.ReferenceSet, out var documentRefSetProperty))
            {
                document.DocumentRefs.ParseAndAdd(documentRefSetProperty);
            }

            return document;
        }
    }
}