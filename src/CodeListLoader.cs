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
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenCodeList;

/// <summary>
/// A generic loader for code list documents and code list set documents. The loader will automatically detect the 
/// document type and return the appropriate instance.
/// </summary>
public static class CodeListLoader
{
    /// <summary>
    /// Loads a new document from a stream. The stream data must be formtted according to the OpenCodeList JSON 
    /// schema specification.
    /// </summary>
    /// <param name="stream">The input stream</param>
    /// <returns>Either a new <see cref="CodeListDocument"/> or a new <see cref="CodeListSetDocument"/> instance</returns>
    public static CodeListBase Load(Stream stream)
    {
        using var jsonDocument = JsonDocument.Parse(stream, default);

        return Parse(jsonDocument.RootElement);
    }

    /// <summary>
    /// Loads a new document from a file. The stream data must be formtted according to the OpenCodeList JSON 
    /// schema specification.
    /// </summary>
    /// <param name="fileInfo">The file info</param>
    /// <returns>Either a new <see cref="CodeListDocument"/> or a new <see cref="CodeListSetDocument"/> instance</returns>
    public static CodeListBase Load(FileInfo fileInfo)
    {
        return Load(fileInfo.FullName);
    }

    /// <summary>
    /// Loads a new document from a file. The stream data must be formtted according to the OpenCodeList JSON 
    /// schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <returns>Either a new <see cref="CodeListDocument"/> or a new <see cref="CodeListSetDocument"/> instance</returns>
    public static CodeListBase Load(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);

        return Load(fileStream);
    }

    /// <summary>
    /// Loads a new document from a stream. The stream data must be formtted according to the OpenCodeList JSON 
    /// schema specification.
    /// </summary>
    /// <param name="stream">The input stream</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous load operation. Returns either a new <see cref="CodeListDocument"/> or 
    /// a new <see cref="CodeListSetDocument"/> instance</returns>
    public static async Task<CodeListBase> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var jsonDocument = await JsonDocument.ParseAsync(stream, default, cancellationToken);

        return Parse(jsonDocument.RootElement);
    }

    /// <summary>
    /// Loads a new document from a file. The stream data must be formtted according to the OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file info</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous load operation. Returns either a new <see cref="CodeListDocument"/> or 
    /// a new <see cref="CodeListSetDocument"/> instance</returns>
    public static Task<CodeListBase> LoadAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        return LoadAsync(fileInfo.FullName, cancellationToken);
    }

    /// <summary>
    /// Loads a new document from a file. The stream data must be formtted according to the OpenCodeList JSON 
    /// schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous load operation. Returns either a new <see cref="CodeListDocument"/> or 
    /// a new <see cref="CodeListSetDocument"/> instance</returns>
    public static async Task<CodeListBase> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        using var fileStream = File.OpenRead(filePath);

        return await LoadAsync(fileStream, cancellationToken);
    }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> object into a document.
    /// </summary>
    /// <param name="rootElement">The JSON object</param>
    /// <returns>Either a new <see cref="CodeListDocument"/> or a new <see cref="CodeListSetDocument"/> instance</returns>
    private static CodeListBase Parse(JsonElement rootElement)
    {
        // Must be an object
        if (rootElement.ValueKind != JsonValueKind.Object)
        {
            throw new CodeListParserException("JSON object expected.");
        }

        // Must contain the OpenCodeList version property
        if (!rootElement.TryGetProperty(PropertyNames.OpenCodeList, out var versionProperty))
        {
            throw new CodeListParserException($"JSON property '{PropertyNames.OpenCodeList}' is missing.");
        }

        // The OpenCodeList version property must be a non-empty string
        if (versionProperty.ValueKind != JsonValueKind.String)
        {
            throw new CodeListParserException($"JSON property '{PropertyNames.OpenCodeList}' must be a string.");
        }

        var versionString = versionProperty.GetString();
        if (string.IsNullOrWhiteSpace(versionString))
        {
            throw new CodeListParserException($"JSON property '{PropertyNames.OpenCodeList}' must not be empty.");
        }

        // Validate the OpenCodeList version
        SemanticVersion version;
        try
        {
            version = SemanticVersion.Parse(versionString);
        }
        catch (Exception ex)
        {
            throw new CodeListParserException($"Invalid OpenCodeList version '{versionString}'.", ex);
        }

        if (!CodeListBase.SupportedVersionRange.Satisfies(version))
        {
            throw new CodeListParserException($"OpenCodeList version '{version}' is not supported.");
        }

        // Check if the JSON contains either a CodeList or a CodeListSet
        var hasCodeList = rootElement.TryGetProperty(PropertyNames.CodeList, out var codeListProperty);
        var hasCodeListSet = rootElement.TryGetProperty(PropertyNames.CodeListSet, out var codeListSetProperty);

        if (hasCodeList == hasCodeListSet)
        {
            throw new CodeListParserException($"Exactly one of '{PropertyNames.CodeList}' or '{PropertyNames.CodeListSet}' must be present.");
        }

        // Parse the appropriate document type
        if (hasCodeList)
        {
            if (codeListProperty.ValueKind != JsonValueKind.Object)
            {
                throw new CodeListParserException($"JSON property '{PropertyNames.CodeList}' must be an object.");
            }

            return CodeListDocument.ParseContent(rootElement, codeListProperty);
        }

        if (codeListSetProperty.ValueKind != JsonValueKind.Object)
        {
            throw new CodeListParserException($"JSON property '{PropertyNames.CodeListSet}' must be an object.");
        }

        return CodeListSetDocument.ParseContent(rootElement, codeListSetProperty);
    }
}