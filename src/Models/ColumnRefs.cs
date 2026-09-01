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
using System.Linq;
using System.Text.Json;

namespace OpenCodeList;

/// <summary>
/// The column references of a code list. This is a collection of references to columns defined in 
/// the <see cref="Columns"/> collection.
/// </summary>
public sealed class ColumnRefs : Owned<CodeListDocument>, IEnumerable<Column>
{
    private readonly IList<Column> _columns = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnRefs"/> class.
    /// </summary>
    /// <param name="owner">The owner of the data set</param>
    public ColumnRefs(CodeListDocument owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Number of columns 
    /// </summary>
    public int Count => _columns.Count;

    /// <summary>
    /// List of columns by ID
    /// </summary>
    public Column this[string columnId]
    {
        get
        {
            var i = IndexOf(x => x.Id == columnId);
            if (i != -1)
            {
                return _columns[i];
            }
            else
            {
                throw new ArgumentException($"Column with ID \"{columnId}\" not found");
            }
        }
        set
        {
            var i = IndexOf(x => x.Id == columnId);
            if (i != -1)
            {
                _columns[i] = value;
            }
            else
            {
                throw new ArgumentException($"Column with ID \"{columnId}\" not found");
            }
        }
    }

    /// <summary>
    /// List of columns by index
    /// </summary>
    public Column this[int index]
    {
        get
        {
            return _columns[index];
        }
        set
        {
            _columns[index] = value;
        }
    }

    /// <summary>
    /// Adds a column to the internal column collection
    /// </summary>  
    /// <param name="column">The column to add</param>
    public void Add(Column column)
    {
        ArgumentNullException.ThrowIfNull(column, nameof(column));  

        if (Owner.Columns.Contains(column))
        {
            _columns.Add(column);
        }
        else
        {
            throw new ArgumentException($"Column with ID \"{column.Id}\" not found in the owner's columns.");
        }
    }

    /// <summary>
    /// Removes all columns from the internal column collection
    /// </summary>
    public void Clear()
    {
        _columns.Clear();
    }

    /// <summary>
    /// Does a certain column exist?
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>True, if found</returns>
    public bool Contains(Func<Column, bool> predicate)
    {
        return _columns.Any(predicate);
    }

    /// <summary>
    /// Finds a certain column
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>A <see cref="Column"/> instance or null</returns>
    public Column FindOrDefault(Func<Column, bool> predicate)
    {
        return _columns.FirstOrDefault(predicate);
    }

    /// <summary>
    /// Support for iteration over a <see cref="Column"/> collection
    /// </summary>
    /// <returns>Enumerator</returns>
    IEnumerator<Column> IEnumerable<Column>.GetEnumerator()
    {
        return _columns.GetEnumerator();
    }

    /// <summary>
    /// Support for iteration over a non-generic collection.
    /// </summary>
    /// <returns>Enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _columns.GetEnumerator();
    }

    /// <summary>
    /// Index of a certain column
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>Index if found, otherwise -1</returns>
    public int IndexOf(Func<Column, bool> predicate)
    {
        for (int i = 0; i < _columns.Count; i++)
        {
            if (predicate(_columns[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Removes a column and all bound keys and foreign keys. Also all 
    /// values bound to this column.
    /// </summary>
    /// <param name="column">The column</param>
    public void Remove(Column column)
    {
        ArgumentNullException.ThrowIfNull(column, nameof(column));

        _columns.Remove(column);
    }

    /// <summary>
    /// Tries to find a column
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <param name="column">The <see cref="Column"/> instancce if found</param>
    /// <returns>True, if successfull</returns>
    public bool TryFind(Func<Column, bool> predicate, out Column column)
    {
        column = FindOrDefault(predicate);
        return column != null;
    }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> array into a new <see cref="Column"/> instances
    /// and adds them to the internal collection.
    /// </summary>
    /// <param name="jsonElement">The json array</param>
    internal void ParseAndAdd(JsonElement jsonElement)
    {
        foreach (var jsonArrayElement in jsonElement.EnumerateArray())
        {
            if (jsonArrayElement.ValueKind == JsonValueKind.String)
            {
                if (Owner.Columns.TryFind(x => x.Id == jsonArrayElement.GetString(), out var column))
                {
                    _columns.Add(column);
                }
                else
                {
                    throw new CodeListParserException($"Column Id \"{jsonArrayElement.GetString()}\" not found.");
                }
            }
            else
            {
                throw new CodeListParserException("Invalid JSON element. Expected a string.");
            }
        }
    }
}