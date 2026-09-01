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
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;

namespace OpenCodeList;

/// <summary>
/// Keys of a code list.
/// </summary>
public sealed class Keys : Owned<CodeListDocument>, IEnumerable<Key>
{
    private readonly List<Key> _keys = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Keys"/> class.
    /// </summary>
    /// <param name="owner">The owner of the data set</param>
    internal Keys(CodeListDocument owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Number of keys 
    /// </summary>
    public int Count => _keys.Count;

    /// <summary>
    /// List of keys by ID
    /// </summary>
    public Key this[string keyId]
    {
        get
        {
            var i = IndexOf(x => x.Id == keyId);
            if (i != -1)
            {
                return _keys[i];
            }
            else
            {
                throw new ArgumentException($"Key with ID \"{keyId}\" not found");
            }
        }
    }

    /// <summary>
    /// List of keys by index
    /// </summary>
    public Key this[int index]
    {
        get
        {
            return _keys[index];
        }
    }

    /// <summary>
    /// Creates a new and empty key and adds it to the internal key collection
    /// </summary>
    /// <returns>the new <see cref="Key"/> instance</returns>
    public Key Add()
    {
        var row = new Key(Owner);
        _keys.Add(row);
        return row;
    }

    /// <summary>
    /// Removes all keys from the internal key collection
    /// </summary>
    public void Clear()
    {
        if (Owner.DefaultKey != null && _keys.Contains(Owner.DefaultKey))
        {
            Owner.DefaultKey = null;
        }

        _keys.Clear();
    }

    /// <summary>
    /// Does a certain key exist?
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>TRue, if found</returns>
    public bool Contains(Func<Key, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return _keys.Any(predicate);
    }

    /// <summary>
    /// Find a certain key
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>A <see cref="Key"/> instance or null</returns>
    public Key FindOrDefault(Func<Key, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return _keys.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Support for iteration over a <see cref="Key"/> collection
    /// </summary>
    /// <returns>Enumerator</returns>
    IEnumerator<Key> IEnumerable<Key>.GetEnumerator()
    {
        return _keys.GetEnumerator();
    }

    /// <summary>
    /// Support for iteration over a non-generic collection.
    /// </summary>
    /// <returns>Enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _keys.GetEnumerator();
    }

    /// <summary>
    /// Index of a key
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>Index if found, otherwise -1</returns>
    public int IndexOf(Func<Key, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate); 
        
        return _keys.FindIndex(key => predicate(key));
    }

    /// <summary>
    /// Removes a key
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>TRUE, if successfull</returns>
    public bool Remove(Key key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (!_keys.Remove(key)) 
        { 
            return false; 
        }

        if (key == Owner.DefaultKey)
        {
            Owner.DefaultKey = null;
        }

        return true;
    }

    /// <summary>
    /// Removes all keys with reference to a given column
    /// </summary>
    /// <param name="column">The column</param>
    /// <returns>Number of removed keys</returns>
    public int RemoveAll(Column column)
    {
        ArgumentNullException.ThrowIfNull(column);

        var removedCount = _keys.RemoveAll(key => key.Columns.Contains(column)); 
        
        if (Owner.DefaultKey is not null && !_keys.Contains(Owner.DefaultKey)) 
        { 
            Owner.DefaultKey = null; 
        }
        
        return removedCount;
    }

    /// <summary>
    /// Tries to find a certain key
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <param name="key">The <see cref="Key"/> instancce if found</param>
    /// <returns>True, if successfull</returns>
    public bool TryFind(Func<Key, bool> predicate, out Key key)
    {
        key = FindOrDefault(predicate);
        
        return key is not null;
    }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> array into a new <see cref="Key"/> instances
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
                _keys.Add(Key.Parse(Owner, jsonArrayElement));
            }
            else
            {
                throw new CodeListParserException("Invalid JSON element. Expected an object.");
            }
        }
    }
}