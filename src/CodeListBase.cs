#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    OpenCodeList.NET 
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License. 
 * 
 */
#endregion

using Enbrea.SemVer;
using FluentValidation.Results;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenCodeList;

/// <summary>
/// An abstract OpenCodeList base class for <see cref="CodeListDocument"/> and
/// <see cref="CodeListSetDocument"/> 
/// </summary>
public abstract class CodeListBase
{
    internal static readonly JsonSerializerOptions JsonSerializerOptions = new() { AllowOutOfOrderMetadataProperties = true };

    /// <summary>
    /// Returns the implemented OpenCodeList version.
    /// </summary>
    /// <returns>A semantic version</returns>
    public static SemanticVersion ImplementedVersion { get; } = new(0, 4, 0, null);

    /// <summary>
    /// Returns the minimum compatible OpenCodeList version.
    /// </summary>
    /// <returns>A semantic version</returns>
    public static SemanticVersion MinimumCompatibleVersion { get; } = new(0, 4, 0, null);

    /// <summary>
    /// Returns the supported OpenCodeList version range.
    /// </summary>
    /// <returns>A semantic version range representing the supported version range.</returns>
    public static SemanticVersionRange SupportedVersionRange { get; } = new(MinimumCompatibleVersion, true, new SemanticVersion(0, 5, 0, null), false);

    /// <summary>
    /// Returns the implemented OpenCodeList version as string.
    /// </summary>
    /// <returns>The implemented OpenCodeList version as string</returns>
    public static string Version { get; } = ImplementedVersion.ToString();

    /// <summary>
    /// Annotations for the document.
    /// </summary>
    public Annotation Annotation { get; set; }

    /// <summary>
    /// Comments for the document.
    /// </summary>
    public IList<string> Comments { get; } = [];

    /// <summary>
    /// Meta information about the document.
    /// </summary>
    public Identification Identification { get; internal set; } = new Identification();

    /// <summary>
    /// TRUE, if this document is a meta document
    /// </summary>
    public bool MetaOnly { get; internal set; }

    /// <summary>
    /// Clears the metadata and content of this document instance.
    /// </summary>
    public virtual void Clear()
    {
        Annotation = null;
        Comments?.Clear();
        Identification.Clear();
        ClearContent(false);
    }

    /// <summary>
    /// Clears only the content of this document instance.
    /// </summary>
    /// <param name="convertToMetaOnly">If TRUE, marks document as meta document</param>
    public virtual void ClearContent(bool convertToMetaOnly)
    {
        if (convertToMetaOnly) MetaOnly = true;
    }

    /// <summary>
    /// Saves this document to a stream according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The output stream</param>
    public void Save(Stream stream)
    {
        Save(stream, true);
    }

    /// <summary>
    /// Saves this document to a stream according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The output stream</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    public void Save(Stream stream, bool indent)
    {
        using var jsonWriter = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = indent });

        jsonWriter.WritDocument(this, MetaOnly);
        jsonWriter.Flush();
    }

    /// <summary>
    /// Saves this document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file path</param>
    public void Save(FileInfo fileInfo)
    {
        using var fileStream = fileInfo.Create();

        Save(fileStream, true);
    }

    /// <summary>
    /// Saves this document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file path</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    public void Save(FileInfo fileInfo, bool indent)
    {
        Save(fileInfo.FullName, indent);
    }

    /// <summary>
    /// Saves this document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    public void Save(string filePath)
    {
        using var fileStream = File.Create(filePath);

        Save(fileStream, true);
    }

    /// <summary>
    /// Saves this document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    public void Save(string filePath, bool indent)
    {
        using var fileStream = File.Create(filePath);

        Save(fileStream, indent);
    }

    /// <summary>
    /// Saves this document as meta document to a stream according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The output stream</param>
    public void SaveAsMetaOnly(Stream stream)
    {
        SaveAsMetaOnly(stream, true);
    }

    /// <summary>
    /// Saves this document as meta document to a stream according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The output stream</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    public void SaveAsMetaOnly(Stream stream, bool indent)
    {
        using var jsonWriter = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = indent });

        jsonWriter.WritDocument(this, true);
        jsonWriter.Flush();
    }

    /// <summary>
    /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file path</param>
    public void SaveAsMetaOnly(FileInfo fileInfo)
    {
        using var fileStream = fileInfo.Create();

        SaveAsMetaOnly(fileStream, true);
    }

    /// <summary>
    /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file path</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    public void SaveAsMetaOnly(FileInfo fileInfo, bool indent)
    {
        SaveAsMetaOnly(fileInfo.FullName, indent);
    }

    /// <summary>
    /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    public void SaveAsMetaOnly(string filePath)
    {
        using var fileStream = File.Create(filePath);

        SaveAsMetaOnly(fileStream, true);
    }

    /// <summary>
    /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    public void SaveAsMetaOnly(string filePath, bool indent)
    {
        using var fileStream = File.Create(filePath);

        SaveAsMetaOnly(fileStream, indent);
    }

    /// <summary>
    /// Saves this document as meta document to a stream according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The output stream</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task reresenting the asynchronous save operation.</returns>
    public Task SaveAsMetaOnlyAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        return SaveAsMetaOnlyAsync(stream, true, cancellationToken);
    }

    /// <summary>
    /// Saves this document as meta document to a stream according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The output stream</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous save operation.</returns>
    public async Task SaveAsMetaOnlyAsync(Stream stream, bool indent, CancellationToken cancellationToken = default)
    {
        using var jsonWriter = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = indent });

        jsonWriter.WritDocument(this, true);

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

        await SaveAsMetaOnlyAsync(fileStream, true, cancellationToken);
    }

    /// <summary>
    /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file info</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous save operation.</returns>
    public Task SaveAsMetaOnlyAsync(FileInfo fileInfo, bool indent, CancellationToken cancellationToken = default)
    {
        return SaveAsMetaOnlyAsync(fileInfo.FullName, indent, cancellationToken);
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

        await SaveAsMetaOnlyAsync(fileStream, true, cancellationToken);
    }

    /// <summary>
    /// Saves this document as meta document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous save operation.</returns>
    public async Task SaveAsMetaOnlyAsync(string filePath, bool indent, CancellationToken cancellationToken = default)
    {
        using var fileStream = File.Create(filePath);

        await SaveAsMetaOnlyAsync(fileStream, indent, cancellationToken);
    }

    /// <summary>
    /// Saves this document to a stream according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The output stream</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task reresenting the asynchronous save operation.</returns>
    public Task SaveAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        return SaveAsync(stream, true, cancellationToken);
    }

    /// <summary>
    /// Saves this document to a stream according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The output stream</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous save operation.</returns>
    public async Task SaveAsync(Stream stream, bool indent, CancellationToken cancellationToken = default)
    {
        using var jsonWriter = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = indent });

        jsonWriter.WritDocument(this, MetaOnly);

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

        await SaveAsync(fileStream, true, cancellationToken);
    }

    /// <summary>
    /// Saves this document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file info</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous save operation.</returns>
    public Task SaveAsync(FileInfo fileInfo, bool indent, CancellationToken cancellationToken = default)
    {
        return SaveAsync(fileInfo.FullName, indent, cancellationToken);
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

        await SaveAsync(fileStream, true, cancellationToken);
    }

    /// <summary>
    /// Saves this document to a file according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <param name="indent">TRUE, if the JSON output should be indented</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous save operation.</returns>
    public async Task SaveAsync(string filePath, bool indent, CancellationToken cancellationToken = default)
    {
        using var fileStream = File.Create(filePath);

        await SaveAsync(fileStream, indent, cancellationToken);
    }

    /// <summary>
    /// Validates this document according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <returns>A <see cref="ValidationResult"/> representing the result of the validation.</returns>  
    public abstract ValidationResult Validate();
}