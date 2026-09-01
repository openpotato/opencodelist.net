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
/// The data rows of a code list.
/// </summary>
public class Rows : Owned<CodeListDocument>, IEnumerable<Row>
{
    private readonly List<Row> _rows = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Rows"/> class.
    /// </summary>
    /// <param name="owner">The owner of the data set</param>
    internal Rows(CodeListDocument owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Number of rows 
    /// </summary>
    public int Count => _rows.Count;

    /// <summary>
    /// List of rows by index
    /// </summary>
    public Row this[int index]
    {
        get
        {
            return _rows[index];
        }
    }

    /// <summary>
    /// Creates a new and empty row and adds it to the internal row collection
    /// </summary>
    /// <returns>The new row</returns>
    public Row Add()
    {
        var row = new Row(Owner);
        Add(row);
        return row;
    }

    /// <summary>
    /// Removes all rows from the internal row collection
    /// </summary>
    public void Clear()
    {
        _rows.Clear();
    }

    /// <summary>
    /// Support for iteration over a <see cref="Rows"/> collection
    /// </summary>
    /// <returns></returns>
    IEnumerator<Row> IEnumerable<Row>.GetEnumerator()
    {
        return _rows.GetEnumerator();
    }

    /// <summary>
    /// Support for iteration over a non-generic collection.
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _rows.GetEnumerator();
    }

    /// <summary>
    /// Removes all values with reference to a given column
    /// </summary>
    /// <param name="columnId">The column Id</param>
    public void RemoveValues(string columnId)
    {
        ArgumentException.ThrowIfNullOrEmpty(columnId);

        foreach (var row in _rows)
        {
            row.RemoveValue(columnId);
        }
    }

    /// <summary>
    /// Removes all values with reference to a given column
    /// </summary>
    /// <param name="column">The column</param>
    public void RemoveValues(Column column)
    {
        ArgumentNullException.ThrowIfNull(column);

        RemoveValues(column.Id);
    }

    /// <summary>
    /// Adds a new row to the internal row collection
    /// </summary>
    /// <param name="row">A new row</param>
    internal void Add(Row row)
    {
        ArgumentNullException.ThrowIfNull(row);

        _rows.Add(row);

        Owner.MetaOnly = false;
    }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> array into a new <see cref="Row"/> instances
    /// and adds them to the internal collection.
    /// </summary>
    /// <param name="jsonElement">The json array</param>
    internal void ParseAndAdd(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind != JsonValueKind.Array)
        {
            throw new CodeListParserException("Invalid JSON element. Expected an array.");
        }

        Owner.MetaOnly = false;

        foreach (var jsonArrayElement in jsonElement.EnumerateArray())
        {
            if (jsonArrayElement.ValueKind == JsonValueKind.Object)
            {
                Add(Row.Parse(Owner, jsonArrayElement));
            }
            else
            {
                throw new CodeListParserException("Invalid JSON element. Expected an object.");
            }
        }
    }
}
