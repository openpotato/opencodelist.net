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
    /// A code list document according to the OpenCodeList specification
    /// </summary>
    public class CodeListDocument : Document 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeListDocument"/> class.
        /// </summary>
        public CodeListDocument()
            : base()
        {
            Columns = new Columns(this);
            Keys = new Keys(this);
            ForeignKeys = new ForeignKeys(this);
            Rows = new Rows(this);
        }

        /// <summary>
        /// The column set of the code list.
        /// </summary>
        public Columns Columns { get; }

        /// <summary>
        /// The default key of the code list.
        /// </summary>
        public Key DefaultKey { get; set; }

        /// <summary>
        /// List of foreign keys.
        /// </summary>
        public ForeignKeys ForeignKeys { get; }

        /// <summary>
        /// List of keys.
        /// </summary>
        public Keys Keys { get; }

        /// <summary>
        /// The data rows of the code list.
        /// </summary>
        public Rows Rows { get; }

        /// <summary>
        /// Loads a new code list from a stream. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The input stream</param>
        public static CodeListDocument Load(Stream stream)
        {
            var jsonDocument = JsonDocument.Parse(stream, default);

            return Parse(jsonDocument.RootElement);
        }

        /// <summary>
        /// Loads a new code list from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file info</param>
        public static CodeListDocument Load(FileInfo fileInfo)
        {
            return Load(fileInfo.FullName);
        }

        /// <summary>
        /// Loads a new code list from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        public static CodeListDocument Load(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);

            return Load(fileStream);
        }

        /// <summary>
        /// Loads a new code list from a stream. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The input stream</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous load operation.</returns>
        public static async Task<CodeListDocument> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var jsonDocument = await JsonDocument.ParseAsync(stream, default, cancellationToken);

            return Parse(jsonDocument.RootElement);
        }

        /// <summary>
        /// Loads a new code list from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file info</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous load operation.</returns>
        public static Task<CodeListDocument> LoadAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            return LoadAsync(fileInfo.FullName, cancellationToken);
        }

        /// <summary>
        /// Loads a new code list from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous load operation.</returns>
        public static async Task<CodeListDocument> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            using var fileStream = File.OpenRead(filePath);

            return await LoadAsync(fileStream, cancellationToken);
        }

        /// <summary>
        /// Clears the metadata and content of this document instance
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            DefaultKey = null;
            Columns.Clear();
            Keys.Clear();
            ForeignKeys.Clear();
        }

        /// <summary>
        /// Clears only the content of this document instance
        /// </summary>
        /// <param name="convertToMetaOnly">If TRUE, marks document as meta document</param>
        public override void ClearContent(bool convertToMetaOnly)
        {
            Rows.Clear();
            base.ClearContent(convertToMetaOnly);
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object (representing the complete document) into a code list document
        /// </summary>
        /// <param name="rootElement">The JSON document root object</param>
        /// <returns>A new <see cref="CodeListDocument"/> instance</returns>
        /// <exception cref="CodeListParserException">Syntax error</exception>
        internal static CodeListDocument Parse(JsonElement rootElement)
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

                if (rootElement.GetRequiredObjectProperty(PropertyNames.CodeList, out var codeListProperty))
                {
                    return ParseContent(rootElement, codeListProperty);
                }
                else
                {
                    throw new CodeListParserException($"JSON Property \"{PropertyNames.CodeList}\" missing.");
                }
            }
            else
            {
                throw new CodeListParserException($"JSON Object expected.");
            }
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object (representing the inner part of the document) into a code list document
        /// </summary>
        /// <param name="rootElement">The JSON document root object</param>
        /// <param name="codeListElement">The JSON sub object for the code list</param>
        /// <returns>A new <see cref="CodeListDocument"/> instance</returns>
        internal static CodeListDocument ParseContent(JsonElement rootElement, JsonElement codeListElement)
        {
            var document = new CodeListDocument();

            if (rootElement.TryGetArrayProperty(PropertyNames.Comments, out var commentsProperty))
            {
                foreach (var commentElement in commentsProperty.EnumerateArray())
                {
                    document.Comments.Add(commentElement.GetString());
                }
            }
            if (codeListElement.TryGetObjectProperty(PropertyNames.Annotation, out var annotationProperty))
            {
                document.Annotation = Annotation.Parse(annotationProperty);
            }
            if (codeListElement.GetRequiredObjectProperty(PropertyNames.Identification, out var identificationProperty))
            {
                document.Identification = Identification.Parse(identificationProperty);
            }
            if (codeListElement.TryGetObjectProperty(PropertyNames.ColumnSet, out var columnSetProperty))
            {
                if (columnSetProperty.TryGetArrayProperty(PropertyNames.Columns, out var columnsProperty))
                {
                    document.Columns.ParseAndAdd(columnsProperty);
                }
                if (columnSetProperty.TryGetArrayProperty(PropertyNames.Keys, out var keysProperty))
                {
                    document.Keys.ParseAndAdd(keysProperty);
                }
                if (columnSetProperty.TryGetObjectProperty(PropertyNames.DefaultKey, out var defaultKeyProperty))
                {
                    if (defaultKeyProperty.TryGetStringProperty(PropertyNames.KeyId, out var keyIdProperty))
                    {
                        if (document.Keys.TryFind(x => x.Id == keyIdProperty.GetString(), out var key))
                        {
                            document.DefaultKey = key;
                        }
                        else
                        {
                            throw new CodeListParserException($"Key Id \"{keyIdProperty.GetString()}\" not found.");
                        }
                    }
                }
                if (columnSetProperty.TryGetArrayProperty(PropertyNames.ForeignKeys, out var foreignKeysProperty))
                {
                    document.ForeignKeys.ParseAndAdd(foreignKeysProperty);
                }
            }
            if (codeListElement.TryGetProperty(PropertyNames.DataSet, out var dataSetProperty))
            {
                if (dataSetProperty.TryGetArrayProperty(PropertyNames.Rows, out var rowsProperty))
                {
                    document.Rows.ParseAndAdd(rowsProperty);
                }
            }

            return document;
        }
    }
}