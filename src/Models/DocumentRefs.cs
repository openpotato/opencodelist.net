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
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenCodeList
{
    /// <summary>
    /// An enumerable list of document references.
    /// </summary>
    public class DocumentRefs : IEnumerable<DocumentRef>
    {
        private readonly CodeListSetDocument _document;
        private readonly List<DocumentRef> _documentRefs = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRefs"/> class.
        /// </summary>
        /// <param name="document">The owner of the reference set</param>
        internal DocumentRefs(CodeListSetDocument document)
        {
            _document = document;
        }

        /// <summary>
        /// Number of document references
        /// </summary>
        public int Count => _documentRefs.Count;

        /// <summary>
        /// List of document references by index
        /// </summary>
        public DocumentRef this[int index]
        {
            get
            {
                return _documentRefs[index];
            }
            set
            {
                _documentRefs[index] = value;
            }
        }

        /// <summary>
        /// Creates a new and empty document reference and adds it to the internal collection
        /// </summary>
        /// <typeparam name="T">The type of the document reference</typeparam>
        /// <returns>The new document reference</returns>
        public T Add<T>() where T : DocumentRef
        {
            var documentRef = Activator.CreateInstance(typeof(T)) as T;
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
        IEnumerator<DocumentRef> IEnumerable<DocumentRef>.GetEnumerator()
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
        public bool Remove(DocumentRef documentRef)
        {
            return _documentRefs.Remove(documentRef);
        }

        /// <summary>
        /// Adds a new document reference to the internal row collection
        /// </summary>
        /// <param name="documentRef">The new document reference</param>
        internal void Add(DocumentRef documentRef)
        {
            _documentRefs.Add(documentRef);
            _document.MetaOnly = false;
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> array into a new <see cref="DocumentRef"/> instances
        /// and adds them to the internal collection.
        /// </summary>
        /// <param name="jsonElement">The json array</param>
        internal void ParseAndAdd(JsonElement jsonElement)
        {
            foreach (var jsonArrayElement in jsonElement.EnumerateArray())
            {
                if (jsonArrayElement.ValueKind == JsonValueKind.Object)
                {
                    if (jsonArrayElement.TryGetProperty(PropertyNames.Type, out var typeProperty))
                    {
                        if (typeProperty.GetString() == TypeConsts.CodeListRef)
                        {
                            Add(CodeListDocumentRef.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.CodeListSetRef)
                        {
                            Add(CodeListSetDocumentRef.Parse(jsonArrayElement));
                        }
                        else
                        {
                            throw new CodeListParserException($"Unknown column type \"{typeProperty.GetString()}\".");
                        }
                    }
                    else
                    {
                        throw new CodeListParserException($"Type column is missing.");
                    }
                }
            }
        }
    }
}

