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
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenCodeList
{
    /// <summary>
    /// An abstract OpenCodeList base document for <see cref="CodeListDocument"/> and
    /// <see cref="CodeListSetDocument"/> 
    /// </summary>
    public abstract class Document
    {
        private bool _metaOnly = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document()
        {
            Identification = new Identification();
        }

        /// <summary>
        /// Annotations for the document.
        /// </summary>
        public Annotation Annotation { get; set; }

        /// <summary>
        /// Comments for the document.
        /// </summary>
        public IList<string> Comments { get; internal set; } = [];

        /// <summary>
        /// Meta information about the document.
        /// </summary>
        public Identification Identification { get; internal set; }

        /// <summary>
        /// TRUE, if this document is a meta document
        /// </summary>
        public bool MetaOnly
        {
            get
            {
                return _metaOnly;
            }
            internal set
            {
                _metaOnly = value;
            }
        }

        /// <summary>
        /// Returns The supported OpenCodeList version.
        /// </summary>
        /// <returns>A OpenCodeList version</returns>
        public static SemanticVersion GetVersion()
        {
            return new SemanticVersion(0, 2, 0, null);
        }

        /// <summary>
        /// Clears the metadata and content of this document instance
        /// </summary>
        public virtual void Clear()
        {
            Annotation = null;
            Comments.Clear();
            Identification = new Identification();
            ClearContent(false);
        }

        /// <summary>
        /// Clears only the content of this document instance
        /// </summary>
        /// <param name="convertToMetaOnly">If TRUE, marks document as meta document</param>
        public virtual void ClearContent(bool convertToMetaOnly)
        {
            if (convertToMetaOnly) _metaOnly = true;
        }

        /// <summary>
        /// Saves this document to a stream according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The output stream</param>
        public void Save(Stream stream)
        {
            Save(stream, new JsonWriterOptions { Indented = true });
        }

        /// <summary>
        /// Saves this document to a stream according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The output stream</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        public void Save(Stream stream, JsonWriterOptions options)
        {
            using var jsonWriter = new Utf8JsonWriter(stream, options);

            jsonWriter.WriteOpenCodeListDocument(this, MetaOnly);
            jsonWriter.Flush();
        }

        /// <summary>
        /// Saves this document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file path</param>
        public void Save(FileInfo fileInfo)
        {
            using var fileStream = fileInfo.Create();

            Save(fileStream, new JsonWriterOptions { Indented = true });
        }

        /// <summary>
        /// Saves this document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file path</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        public void Save(FileInfo fileInfo, JsonWriterOptions options)
        {
            using var fileStream = fileInfo.Create();

            Save(fileStream, options);
        }

        /// <summary>
        /// Saves this document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        public void Save(string filePath)
        {
            using var fileStream = File.Create(filePath);

            Save(fileStream, new JsonWriterOptions { Indented = true });
        }

        /// <summary>
        /// Saves this document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        public void Save(string filePath, JsonWriterOptions options)
        {
            using var fileStream = File.Create(filePath);

            Save(fileStream, options);
        }

        /// <summary>
        /// Saves this document as meta document to a stream according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The output stream</param>
        public void SaveAsMetaOnly(Stream stream)
        {
            SaveAsMetaOnly(stream, new JsonWriterOptions { Indented = true });
        }

        /// <summary>
        /// Saves this document as meta document to a stream according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The output stream</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        public void SaveAsMetaOnly(Stream stream, JsonWriterOptions options)
        {
            using var jsonWriter = new Utf8JsonWriter(stream, options);

            jsonWriter.WriteOpenCodeListDocument(this, true);
            jsonWriter.Flush();
        }

        /// <summary>
        /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file path</param>
        public void SaveAsMetaOnly(FileInfo fileInfo)
        {
            using var fileStream = fileInfo.Create();

            SaveAsMetaOnly(fileStream, new JsonWriterOptions { Indented = true });
        }

        /// <summary>
        /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file path</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        public void SaveAsMetaOnly(FileInfo fileInfo, JsonWriterOptions options)
        {
            using var fileStream = fileInfo.Create();

            SaveAsMetaOnly(fileStream, options);
        }

        /// <summary>
        /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        public void SaveAsMetaOnly(string filePath)
        {
            using var fileStream = File.Create(filePath);

            SaveAsMetaOnly(fileStream, new JsonWriterOptions { Indented = true });
        }

        /// <summary>
        /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        public void SaveAsMetaOnly(string filePath, JsonWriterOptions options)
        {
            using var fileStream = File.Create(filePath);

            SaveAsMetaOnly(fileStream, options);
        }

        /// <summary>
        /// Saves this document as meta document to a stream according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The output stream</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task reresenting the asynchronous save operation.</returns>
        public async Task SaveAsMetaOnlyAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            await SaveAsMetaOnlyAsync(stream, new JsonWriterOptions { Indented = true }, cancellationToken);
        }

        /// <summary>
        /// Saves this document as meta document to a stream according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The output stream</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        public async Task SaveAsMetaOnlyAsync(Stream stream, JsonWriterOptions options, CancellationToken cancellationToken = default)
        {
            using var jsonWriter = new Utf8JsonWriter(stream, options);

            jsonWriter.WriteOpenCodeListDocument(this, true);

            await jsonWriter.FlushAsync(cancellationToken);
        }

        /// <summary>
        /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file info</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task reresenting the asynchronous save operation.</returns>
        public async Task SaveAsMetaOnlyAsync(FileInfo filePath, CancellationToken cancellationToken = default)
        {
            using var fileStream = filePath.Create();

            await SaveAsMetaOnlyAsync(fileStream, new JsonWriterOptions { Indented = true }, cancellationToken);
        }

        /// <summary>
        /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file info</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        public async Task SaveAsMetaOnlyAsync(FileInfo fileInfo, JsonWriterOptions options, CancellationToken cancellationToken = default)
        {
            using var fileStream = fileInfo.Create();

            await SaveAsMetaOnlyAsync(fileStream, options, cancellationToken);
        }

        /// <summary>
        /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task reresenting the asynchronous save operation.</returns>
        public async Task SaveAsMetaOnlyAsync(string filePath, CancellationToken cancellationToken = default)
        {
            using var fileStream = File.Create(filePath);

            await SaveAsMetaOnlyAsync(fileStream, new JsonWriterOptions { Indented = true }, cancellationToken);
        }

        /// <summary>
        /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        public async Task SaveAsMetaOnlyAsync(string filePath, JsonWriterOptions options, CancellationToken cancellationToken = default)
        {
            using var fileStream = File.Create(filePath);

            await SaveAsMetaOnlyAsync(fileStream, options, cancellationToken);
        }

        /// <summary>
        /// Saves this document to a stream according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The output stream</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task reresenting the asynchronous save operation.</returns>
        public async Task SaveAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            await SaveAsync(stream, new JsonWriterOptions { Indented = true }, cancellationToken);
        }

        /// <summary>
        /// Saves this document to a stream according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="stream">The output stream</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        public async Task SaveAsync(Stream stream, JsonWriterOptions options, CancellationToken cancellationToken = default)
        {
            using var jsonWriter = new Utf8JsonWriter(stream, options);

            jsonWriter.WriteOpenCodeListDocument(this, MetaOnly);
            
            await jsonWriter.FlushAsync(cancellationToken);
        }

        /// <summary>
        /// Saves this document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file info</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task reresenting the asynchronous save operation.</returns>
        public async Task SaveAsync(FileInfo filePath, CancellationToken cancellationToken = default)
        {
            using var fileStream = filePath.Create();

            await SaveAsync(fileStream, new JsonWriterOptions { Indented = true }, cancellationToken);
        }

        /// <summary>
        /// Saves this document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="fileInfo">The file info</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        public async Task SaveAsync(FileInfo fileInfo, JsonWriterOptions options, CancellationToken cancellationToken = default)
        {
            using var fileStream = fileInfo.Create();

            await SaveAsync(fileStream, options, cancellationToken);
        }

        /// <summary>
        /// Saves this document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task reresenting the asynchronous save operation.</returns>
        public async Task SaveAsync(string filePath, CancellationToken cancellationToken = default)
        {
            using var fileStream = File.Create(filePath);

            await SaveAsync(fileStream, new JsonWriterOptions { Indented = true }, cancellationToken);
        }

        /// <summary>
        /// Saves this document to a file according to the OpenCodeList JSON schema specification.
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="options">An instance of <see cref="JsonWriterOptions"/> to customize the behaviour when generating JSON </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the asynchronous save operation.</returns>
        public async Task SaveAsync(string filePath, JsonWriterOptions options, CancellationToken cancellationToken = default)
        {
            using var fileStream = File.Create(filePath);

            await SaveAsync(fileStream, options, cancellationToken);
        }
    }
}