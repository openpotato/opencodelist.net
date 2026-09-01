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
/// The column definitions of a code list.
/// </summary>
public sealed class Columns : Owned<CodeListDocument>, IEnumerable<Column>
{
    private readonly List<Column> _columns = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Columns"/> class.
    /// </summary>
    /// <param name="owner">The owner of the data set</param>
    public Columns(CodeListDocument owner)
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
    }

    /// <summary>
    /// Creates a new and empty column and adds it to the internal column collection
    /// </summary>
    /// <typeparam name="T">Type of column</typeparam>
    /// <returns>The new column instance</returns>
    public T Add<T>() where T : Column, new()
    {
        var column = new T();
        _columns.Add(column);
        return column;
    }

    /// <summary>
    /// Removes all columns from the internal column collection
    /// </summary>
    public void Clear()
    {
        if (Owner.Rows.Count == 0)
        {
            Owner.ForeignKeys.Clear();
            Owner.Keys.Clear();

            _columns.Clear();
        }
        else
        {
            throw new InvalidOperationException("Removing columns from a filled code list is not allowed.");
        }
    }

    /// <summary>
    /// Does a certain column exist?
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>True, if found</returns>
    public bool Any(Func<Column, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        
        return _columns.Any(predicate);
    }

    /// <summary>
    /// Finds a certain column
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <returns>A <see cref="Column"/> instance or null</returns>
    public Column FindOrDefault(Func<Column, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

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
        ArgumentNullException.ThrowIfNull(predicate);

        return _columns.FindIndex(column => predicate(column));
    }

    /// <summary>
    /// Removes a column and all bound keys and foreign keys. Also all 
    /// values bound to this column.
    /// </summary>
    /// <param name="column">The column</param>
    public void Remove(Column column)
    {
        ArgumentNullException.ThrowIfNull(column);

        if (Owner.Rows.Count == 0)
        {
            if (_columns.Remove(column))
            {
                Owner.Keys.RemoveAll(column);
                Owner.ForeignKeys.RemoveAll(column);
                Owner.Rows.RemoveValues(column);
            }
        }
        else
        {
            throw new InvalidOperationException("Removing columns from a filled code list is not allowed.");
        }
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
        if (jsonElement.ValueKind != JsonValueKind.Array) 
        { 
            throw new CodeListParserException("Invalid JSON element. Expected an array."); 
        }

        foreach (var jsonArrayElement in jsonElement.EnumerateArray())
        {
            if (jsonArrayElement.ValueKind == JsonValueKind.Object)
            {
                var column = JsonSerializer.Deserialize<Column>(jsonArrayElement, CodeListBase.JsonSerializerOptions);

               if (column is not null)
                { 
                    _columns.Add(column);
                }
                else
                {
                    throw new CodeListParserException("Could not deserialize column definition."); 
                }
            }
            else
            {
                throw new CodeListParserException("Invalid JSON element. Expected an object.");
            }
        }
    }
}