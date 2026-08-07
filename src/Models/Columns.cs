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
using System.Linq;
using System.Text.Json;

namespace OpenCodeList
{
    /// <summary>
    /// The column definitions of a code list.
    /// </summary>
    public class Columns : IEnumerable<Column>
    {
        private readonly IList<Column> _columns = [];
        private readonly CodeListDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="Columns"/> class.
        /// </summary>
        /// <param name="owner">The owner of the data set</param>
        public Columns(CodeListDocument document)
        {
            _document = document;
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
                    throw new Exception($"Column with ID \"{columnId}\" not found");
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
                    throw new Exception($"Column with ID \"{columnId}\" not found");
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
        /// Creates a new and empty column and adds it to the internal column collection
        /// </summary>
        /// <typeparam name="T">Type of column</typeparam>
        /// <returns>The new column instance</returns>
        public T Add<T>() where T : Column
        {
            var column = Activator.CreateInstance<T>() as T;
            _columns.Add(column);
            return column;
        }

        /// <summary>
        /// Removes all columns from the internal column collection
        /// </summary>
        public void Clear()
        {
            _document.Keys.Clear();
            _document.ForeignKeys.Clear();
            _columns.Clear();
            _document.Rows.Clear();
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
            if (_document.Rows.Count == 0)
            {
                if (_columns.Remove(column))
                {
                    _document.Keys.RemoveAll(column);
                    _document.ForeignKeys.RemoveAll(column);
                    _document.Rows.RemoveValues(column);
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
            foreach (var jsonArrayElement in jsonElement.EnumerateArray())
            {
                // Column reference
                if (jsonArrayElement.ValueKind == JsonValueKind.String)
                {
                    if (_document.Columns.TryFind(x => x.Id == jsonArrayElement.GetString(), out var column))
                    {
                        _columns.Add(column);
                    }
                    else
                    {
                        throw new CodeListParserException($"Column Id \"{jsonArrayElement.GetString()}\" not found.");
                    }
                }

                // Column defintion
                else if (jsonArrayElement.ValueKind == JsonValueKind.Object)
                {
                    if (jsonArrayElement.TryGetProperty(PropertyNames.Type, out var typeProperty))
                    {
                        if (typeProperty.GetString() == TypeConsts.String)
                        {
                            _columns.Add(StringColumn.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.Enum)
                        {
                            _columns.Add(EnumColumn.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.EnumSet)
                        {
                            _columns.Add(EnumSetColumn.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.Number)
                        {
                            _columns.Add(NumberColumn.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.Integer)
                        {
                            _columns.Add(IntegerColumn.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.Boolean)
                        {
                            _columns.Add(BooleanColumn.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.DateOnly)
                        {
                            _columns.Add(DateOnlyColumn.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.DateTime)
                        {
                            _columns.Add(DateTimeColumn.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.TimeOnly)
                        {
                            _columns.Add(TimeOnlyColumn.Parse(jsonArrayElement));
                        }
                        else if (typeProperty.GetString() == TypeConsts.Document)
                        {
                            _columns.Add(JsonColumn.Parse(jsonArrayElement));
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