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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenCodeList;

/// <summary>
/// An enumerable list of document references.
/// </summary>
public sealed class ExternalCodeListBaseRefs : Owned<CodeListSetDocument>, IEnumerable<ExternalCodeListBaseRef>
{
    private readonly List<ExternalCodeListBaseRef> _documentRefs = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalCodeListBaseRefs"/> class.
    /// </summary>
    /// <param name="owner">The owner of the reference set</param>
    internal ExternalCodeListBaseRefs(CodeListSetDocument owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Number of document references
    /// </summary>
    public int Count => _documentRefs.Count;

    /// <summary>
    /// List of document references by index
    /// </summary>
    public ExternalCodeListBaseRef this[int index]
    {
        get
        {
            return _documentRefs[index];
        }
    }

    /// <summary>
    /// Creates a new and empty document reference and adds it to the internal collection
    /// </summary>
    /// <typeparam name="T">The type of the document reference</typeparam>
    /// <returns>The new document reference</returns>
    public T Add<T>() where T : ExternalCodeListBaseRef, new()
    {
        var documentRef = new T();
        Add(documentRef);
        return documentRef;
    }

    /// <summary>
    /// Removes all document references from the internal collection
    /// </summary>
    public void Clear()
    {
        _documentRefs.Clear();
    }

    /// <summary>
    /// Support for iteration over a document reference collection
    /// </summary>
    /// <returns></returns>
    IEnumerator<ExternalCodeListBaseRef> IEnumerable<ExternalCodeListBaseRef>.GetEnumerator()
    {
        return _documentRefs.GetEnumerator();
    }

    /// <summary>
    /// Support for iteration over a non-generic collection.
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _documentRefs.GetEnumerator();
    }

    /// <summary>
    /// Removes the given document reference  from the internal collection
    /// </summary>
    /// <param name="documentRef">The document reference</param>
    /// <returns>True if the document reference was found and successfully removed</returns>
    public bool Remove(ExternalCodeListBaseRef documentRef)
    {
        ArgumentNullException.ThrowIfNull(documentRef);

        return _documentRefs.Remove(documentRef);
    }

    /// <summary>
    /// Adds a new document reference to the internal row collection
    /// </summary>
    /// <param name="documentRef">The new document reference</param>
    internal void Add(ExternalCodeListBaseRef documentRef)
    {
        _documentRefs.Add(documentRef);

        Owner.MetaOnly = false;
    }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> array into a new <see cref="ExternalCodeListBaseRef"/> instances
    /// and adds them to the internal collection.
    /// </summary>
    /// <param name="jsonElement">The json array</param>
    internal void ParseAndAdd(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind != JsonValueKind.Array) 
        { 
            throw new CodeListParserException("Invalid JSON element. Expected an array."); 
        }

        foreach (var jsonArrayElement in jsonElement.EnumerateArray())
        {
            if (jsonArrayElement.ValueKind == JsonValueKind.Object)
            {
                var documentRef = JsonSerializer.Deserialize<ExternalCodeListBaseRef>(jsonArrayElement, CodeListBase.JsonSerializerOptions); 
                
                if (documentRef is not null)
                {
                    Add(documentRef);
                }
                else
                {
                    throw new CodeListParserException("Could not deserialize document reference.");
                }
            }
            else 
            { 
                throw new CodeListParserException("Invalid JSON element. Expected an object.");
            }
        }
    }
}

