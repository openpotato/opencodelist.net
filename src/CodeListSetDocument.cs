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
/// A code list set document according to the OpenCodeList specification
/// </summary>
public class CodeListSetDocument : CodeListBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeListSetDocument"/> class.
    /// </summary>
    public CodeListSetDocument()
        : base()
    {
        DocumentRefs = new ExternalCodeListBaseRefs(this);
    }

    /// <summary>
    /// The list of document references.
    /// </summary>
    public ExternalCodeListBaseRefs DocumentRefs { get; }

    /// <summary>
    /// Loads a new code list set from a stream. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// /// </summary>
    /// <param name="stream">The input stream</param>
    public static CodeListSetDocument Load(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var jsonDocument = JsonDocument.Parse(stream, default);

        return Parse(jsonDocument.RootElement);
    }

    /// <summary>
    /// Loads a new code list set from a file. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file info</param>
    public static CodeListSetDocument Load(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        return Load(fileInfo.FullName);
    }

    /// <summary>
    /// Loads a new code list set from a file. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    public static CodeListSetDocument Load(string filePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        using var fileStream = File.OpenRead(filePath);

        return Load(fileStream);
    }

    /// <summary>
    /// Loads a new code list set from a stream. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The input stream</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous load operation.</returns>
    public static async Task<CodeListSetDocument> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var jsonDocument = await JsonDocument.ParseAsync(stream, default, cancellationToken);

        return Parse(jsonDocument.RootElement);
    }

    /// <summary>
    /// Loads a new code list set from a file. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file info</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous load operation.</returns>
    public static Task<CodeListSetDocument> LoadAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        return LoadAsync(fileInfo.FullName, cancellationToken);
    }

    /// <summary>
    /// Loads a new code list set from a file. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous load operation.</returns>
    public static async Task<CodeListSetDocument> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

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
    /// Validates the code list set document according to the OpenCodeList specification
    /// </summary>
    public override void Validate()
    {
        var validator = new CodeListDocumentSetValidator(this);
        validator.Validate();
    }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> object (representing the complete document) into a code list set document
    /// </summary>
    /// <param name="rootElement">The JSON document root object</param>
    /// <returns>A new <see cref="CodeListSetDocument"/> instance</returns>
    internal static CodeListSetDocument Parse(JsonElement rootElement)
    {
        if (rootElement.ValueKind != JsonValueKind.Object)
        {
            throw new CodeListParserException("Invalid JSON document.Expected a root object.");
        }

        if (!rootElement.TryGetProperty(PropertyNames.OpenCodeList, out var versionProperty))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.OpenCodeList}' is missing.");
        }

        if (versionProperty.ValueKind != JsonValueKind.String)
        {
            throw new CodeListParserException($"JSON property '{PropertyNames.OpenCodeList}' must be a string.");
        }

        var versionString = versionProperty.GetString();

        if (string.IsNullOrWhiteSpace(versionString))
        {
            throw new CodeListParserException($"JSON property '{PropertyNames.OpenCodeList}' must not be empty.");
        }

        SemanticVersion version;
        try
        {
            version = SemanticVersion.Parse(versionString);
        }
        catch (Exception ex)
        {
            throw new CodeListParserException($"Invalid OpenCodeList version '{versionString}'.", ex);
        }

        if (!SupportedVersionRange.Satisfies(version))
        {
            throw new CodeListParserException($"OpenCodeList version '{version}' is not supported.");
        }

        if (!rootElement.GetRequiredObjectProperty(PropertyNames.CodeListSet, out var codeListSetProperty))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.CodeListSet}' is missing.");
        }

        return ParseContent(rootElement, codeListSetProperty);
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

        ParseComments(document, rootElement);
        ParseAnnotation(document, codeListSetElement);
        ParseIdentification(document, codeListSetElement);
        ParseReferenceSet(document, codeListSetElement);

        return document;
    }

    /// <summary>
    /// Parses the annotation from the code list set JSON element and adds it to the code list set document
    /// </summary>
    private static void ParseAnnotation(CodeListSetDocument owner, JsonElement codeListSetElement)
    {
        if (!codeListSetElement.TryGetObjectProperty(PropertyNames.Annotation, out var annotationElement))
        {
            return;
        }

        owner.Annotation = JsonSerializer.Deserialize<Annotation>(annotationElement, JsonSerializerOptions);
    }

    /// <summary>
    /// Parses the comments from the code list set JSON element and adds them to the code list set document
    /// </summary>
    private static void ParseComments(CodeListSetDocument owner, JsonElement codeListSetElement)
    {
        if (!codeListSetElement.TryGetArrayProperty(PropertyNames.Comments, out var commentsElement))
        {
            return;
        }

        foreach (var element in commentsElement.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.String)
            {
                throw new CodeListParserException($"Invalid JSON element in '{PropertyNames.Comments}'. Expected a string.");
            }

            owner.Comments.Add(element.GetString()!);
        }
    }

    /// <summary>
    /// Parses the identification from the code list set JSON element and adds it to the code list set document
    /// </summary>
    private static void ParseIdentification(CodeListSetDocument document, JsonElement codeListSetElement)
    {
        if (!codeListSetElement.GetRequiredObjectProperty(PropertyNames.Identification, out var identificationElement))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.Identification}' is missing.");
        }

        document.Identification = JsonSerializer.Deserialize<Identification>(identificationElement, JsonSerializerOptions);
    }

    /// <summary>
    /// Parses the reference set from the code list set JSON element and adds it to the code list set document
    /// </summary>
    private static void ParseReferenceSet(CodeListSetDocument document, JsonElement codeListSetElement)
    {
        if (!codeListSetElement.TryGetArrayProperty(PropertyNames.ReferenceSet, out var documentRefSetProperty))
        {
            document.MetaOnly = true;
            return;
        }

        document.MetaOnly = false;
        document.DocumentRefs.ParseAndAdd(documentRefSetProperty);
    }
}