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
using FluentValidation;
using FluentValidation.Results;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenCodeList;

/// <summary>
/// A code list document according to the OpenCodeList specification
/// </summary>
public class CodeListDocument : CodeListBase
{
    private Key _defaultKey;

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
    public Key DefaultKey
    {
        get
        {
            return _defaultKey;
        }
        set
        {
            if (value is not null && !Keys.Contains(key => ReferenceEquals(key, value)))
            {
                throw new ArgumentException("The default key must belong to this document.", nameof(value));
            }

            _defaultKey = value;
        }
    }

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
    /// Loads a new code list from a stream. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The input stream</param>
    public static CodeListDocument Load(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var jsonDocument = JsonDocument.Parse(stream, default);

        return Parse(jsonDocument.RootElement);
    }

    /// <summary>
    /// Loads a new code list from a file. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file info</param>
    public static CodeListDocument Load(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        return Load(fileInfo.FullName);
    }

    /// <summary>
    /// Loads a new code list from a file. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    public static CodeListDocument Load(string filePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        using var fileStream = File.OpenRead(filePath);

        return Load(fileStream);
    }

    /// <summary>
    /// Loads a new code list from a stream. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="stream">The input stream</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous load operation.</returns>
    public static async Task<CodeListDocument> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var jsonDocument = await JsonDocument.ParseAsync(stream, default, cancellationToken);

        return Parse(jsonDocument.RootElement);
    }

    /// <summary>
    /// Loads a new code list from a file. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="fileInfo">The file info</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous load operation.</returns>
    public static Task<CodeListDocument> LoadAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        return LoadAsync(fileInfo.FullName, cancellationToken);
    }

    /// <summary>
    /// Loads a new code list from a file. The stream data must be formtted according to the 
    /// OpenCodeList JSON schema specification.
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous load operation.</returns>
    public static async Task<CodeListDocument> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

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
        Keys?.Clear();
        ForeignKeys?.Clear();
        Columns?.Clear();
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
    /// Validates the code list document according to the OpenCodeList specification
    /// </summary>
    /// <returns>A <see cref="ValidationResult"/> representing the result of the validation.</returns>  
    public override ValidationResult Validate()
    {
        var validator = new CodeListDocumentValidator();
        return validator.Validate(this);
    }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> object (representing the complete document) into a code list document
    /// </summary>
    /// <param name="rootElement">The JSON document root object</param>
    /// <returns>A new <see cref="CodeListDocument"/> instance</returns>
    /// <exception cref="CodeListParserException">Syntax error</exception>
    internal static CodeListDocument Parse(JsonElement rootElement)
    {
        if (rootElement.ValueKind != JsonValueKind.Object)
        {
            throw new CodeListParserException("Invalid JSON document. Expected a root object.");
        }

        if (!rootElement.TryGetProperty(PropertyNames.OpenCodeList, out var versionProperty))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.OpenCodeList}' is missing.");
        }

        if (versionProperty.ValueKind != JsonValueKind.String)
        {
            throw new CodeListParserException($"JSON property '{PropertyNames.OpenCodeList}' must be a string.");
        }

        OpenCodeListVersion.Parse(versionProperty.GetString());

        if (!rootElement.GetRequiredObjectProperty(PropertyNames.CodeList, out var codeListProperty))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.CodeList}' is missing.");
        }

        return ParseContent(rootElement, codeListProperty);
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

        ParseComments(document, rootElement);
        ParseAnnotation(document, codeListElement);
        ParseIdentification(document, codeListElement);
        ParseColumnSet(document, codeListElement);
        ParseDataSet(document, codeListElement);

        return document;
    }

    /// <summary>
    /// Parses the annotation from the code list JSON element and adds it to the code list document
    /// </summary>
    private static void ParseAnnotation(CodeListDocument owner, JsonElement codeListElement)
    {
        if (!codeListElement.TryGetObjectProperty(PropertyNames.Annotation, out var annotationElement))
        {
            return;
        }

        owner.Annotation = JsonSerializer.Deserialize<Annotation>(annotationElement, JsonSerializerOptions);
    }

    /// <summary>
    /// Parses the column set from the code list JSON element and adds it to the code list document
    /// </summary>
    private static void ParseColumnSet(CodeListDocument document, JsonElement codeListElement)
    {
        if (!codeListElement.GetRequiredObjectProperty(PropertyNames.ColumnSet, out var columnSetElement))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.ColumnSet}' is missing.");
        }

        if (!columnSetElement.GetRequiredArrayProperty(PropertyNames.Columns, out var columnsElement))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.Columns}' is missing.");
        }

        document.Columns.ParseAndAdd(columnsElement);

        if (!columnSetElement.GetRequiredArrayProperty(PropertyNames.Keys, out var keysElement))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.Keys}' is missing.");
        }

        document.Keys.ParseAndAdd(keysElement);

        ParseDefaultKey(document, columnSetElement);

        if (columnSetElement.TryGetArrayProperty(PropertyNames.ForeignKeys, out var foreignKeysElement))
        {
            document.ForeignKeys.ParseAndAdd(foreignKeysElement);
        }
    }

    /// <summary>
    /// Parses the comments from the root JSON element and adds them to the code list document
    /// </summary>
    private static void ParseComments(CodeListDocument owner, JsonElement rootElement)
    {
        if (!rootElement.TryGetArrayProperty(PropertyNames.Comments, out var commentsElement))
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
    /// Parses the data set from the code list JSON element and adds it to the code list document
    /// </summary>
    private static void ParseDataSet(CodeListDocument document, JsonElement codeListElement)
    {
        if (!codeListElement.TryGetObjectProperty(PropertyNames.DataSet, out var dataSetElement))
        {
            document.MetaOnly = true;
            return;
        }

        if (!dataSetElement.GetRequiredArrayProperty(PropertyNames.Rows, out var rowsElement))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.Rows}' is missing.");
        }

        document.Rows.ParseAndAdd(rowsElement);
    }

    /// <summary>
    /// Parses the default key from the column set JSON element and adds it to the code list document
    /// </summary>
    private static void ParseDefaultKey(CodeListDocument document, JsonElement columnSetElement)
    {
        if (!columnSetElement.TryGetObjectProperty(PropertyNames.DefaultKey, out var defaultKeyElement))
        {
            return;
        }

        if (!defaultKeyElement.GetRequiredStringProperty(PropertyNames.KeyId, out var keyIdElement))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.KeyId}' is missing.");
        }

        var keyId = keyIdElement.GetString();

        if (!document.Keys.TryFind(key => key.Id == keyId, out var key))
        {
            throw new CodeListParserException($"Key with ID \"{keyId}\" was not found.");
        }

        document.DefaultKey = key;
    }

    /// <summary>
    /// Parses the identification from the code list JSON element and adds it to the code list document
    /// </summary>
    private static void ParseIdentification(CodeListDocument document, JsonElement codeListElement)
    {
        if (!codeListElement.GetRequiredObjectProperty(PropertyNames.Identification, out var identificationElement))
        {
            throw new CodeListParserException($"Required JSON property '{PropertyNames.Identification}' is missing.");
        }

        document.Identification = JsonSerializer.Deserialize<Identification>(identificationElement, JsonSerializerOptions);
    }
}
