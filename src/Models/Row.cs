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
using System.Text.Json.Nodes;

namespace OpenCodeList
{
    /// <summary>
    /// A data row of a code list.
    /// </summary>
    public class Row : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly CodeListDocument _document;
        private readonly Dictionary<string, object> _values = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        /// <param name="document">The owner of the data set</param>
        public Row(CodeListDocument document)
        {
            _document = document;
            ClearValues();
        }

        /// <summary>
        /// List of values by column id
        /// </summary>
        public object this[string columnId]
        {
            get
            {
                if (_values.TryGetValue(columnId, out var value))
                {
                    return value;
                }
                else
                {
                    if (_document.Columns.Contains(x => x.Id == columnId))
                    {
                        return null;
                    }
                    else
                    {
                        throw new CodeListParserException($"Column with ID \"{columnId}\" not found");
                    }
                }
            }
            set
            {
                if (_document.Columns.TryFind(x => x.Id == columnId, out var column))
                {
                    AssignValue(column, value);
                }
                else
                {
                    throw new CodeListParserException($"Column with ID \"{columnId}\" not found");
                }
            }
        }

        /// <summary>
        /// Removes all values of the row.
        /// </summary>
        public void ClearValues()
        {
            _values.Clear();
        }

        /// <summary>
        /// Support for iteration over a <see cref="KeyValuePair<string, object>"/> collection
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// Support for iteration over a non-generic collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// Removes the value with reference to a given column
        /// </summary>
        /// <param name="columnId">The column Id</param>
        /// <returns>TRUE, if removed</returns>
        public bool RemoveValue(string columnId)
        {
            return _values.Remove(columnId);
        }

        /// <summary>
        /// Removes the value with reference to a given column
        /// </summary>
        /// <param name="column">The column</param>
        /// <returns>TRUE, if removed</returns>
        public bool RemoveValue(Column column)
        {
            return RemoveValue(column.Id);
        }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="Row"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="Row"/> instance</returns>
        internal static Row Parse(JsonElement jsonElement, CodeListDocument codeList)
        {
            var row = new Row(codeList);

            foreach (var jsonObjectProperty in jsonElement.EnumerateObject())
            {
                if (row._document.Columns.TryFind(x => x.Id == jsonObjectProperty.Name, out var column))
                {
                    if (jsonObjectProperty.Value.ValueKind != JsonValueKind.Null)
                    {
                        if (column is StringColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.String)
                            {
                                row._values[column.Id] = jsonObjectProperty.Value.GetString();
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not a string.");
                            }
                        }
                        else if (column is BooleanColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.True ||
                                jsonObjectProperty.Value.ValueKind == JsonValueKind.False)
                            {
                                row._values[column.Id] = jsonObjectProperty.Value.GetBoolean();
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not a boolean.");
                            }
                        }
                        else if (column is IntegerColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Number)
                            {
                                row._values[column.Id] = jsonObjectProperty.Value.GetInt32();
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not an integer.");
                            }
                        }
                        else if (column is NumberColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Number)
                            {
                                row._values[column.Id] = jsonObjectProperty.Value.GetDecimal();
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not a number.");
                            }
                        }
                        else if (column is DateTimeColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.String)
                            {
                                row._values[column.Id] = DateTimeUtils.ParseDateTimeOffset(jsonObjectProperty.Value.GetString());
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not a date-time.");
                            }
                        }
                        else if (column is DateOnlyColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.String)
                            {
                                row._values[column.Id] = DateTimeUtils.ParseDateOnly(jsonObjectProperty.Value.GetString());
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not a date.");
                            }
                        }
                        else if (column is TimeOnlyColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.String)
                            {
                                row._values[column.Id] = DateTimeUtils.ParseTimeOnly(jsonObjectProperty.Value.GetString());
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not a time.");
                            }
                        }
                        else if (column is EnumColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.String)
                            {
                                row._values[column.Id] = jsonObjectProperty.Value.GetString();
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not a enum.");
                            }
                        }
                        else if (column is EnumSetColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Array)
                            {
                                var stringList = new List<string>();
                                foreach (var enumStringProperty in jsonObjectProperty.Value.EnumerateArray())
                                {
                                    if (enumStringProperty.ValueKind == JsonValueKind.String)
                                    {
                                        stringList.Add(enumStringProperty.GetString());
                                    }
                                    else
                                    {
                                        throw new CodeListParserException($"Value is not a enum-set.");
                                    }
                                }
                                row._values[column.Id] = stringList;
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not an array.");
                            }
                        }
                        else if (column is JsonColumn)
                        {
                            if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Object)
                            {
                                row._values[column.Id] = JsonObject.Create(jsonObjectProperty.Value);
                            }
                            else if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Array)
                            {
                                row._values[column.Id] = JsonArray.Create(jsonObjectProperty.Value);
                            }
                            else
                            {
                                throw new CodeListParserException($"Value is not an object or an array.");
                            }
                        }
                    }
                }
                else
                {
                    throw new CodeListParserException($"Column with Id \"{jsonObjectProperty.Name}\" not found.");
                }
            }

            return row;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            var cellIndex = 0;

            jsonWriter.WriteStartObject();

            foreach (var column in _document.Columns)
            {
                if (_values.TryGetValue(column.Id, out var value))
                {
                    if (_document.Columns[cellIndex] is StringColumn)
                    {
                        if (value is string stringValue)
                        {
                            jsonWriter.WriteString(_document.Columns[cellIndex].Id, stringValue);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is not a string.");
                        }
                    }
                    else if (_document.Columns[cellIndex] is BooleanColumn)
                    {
                        if (value is bool boolValue)
                        {
                            jsonWriter.WriteBoolean(_document.Columns[cellIndex].Id, boolValue);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is not a bool.");
                        }
                    }
                    else if (_document.Columns[cellIndex] is IntegerColumn)
                    {
                        if (value is int integerValue)
                        {
                            jsonWriter.WriteNumber(_document.Columns[cellIndex].Id, integerValue);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is not an int.");
                        }
                    }
                    else if (_document.Columns[cellIndex] is NumberColumn)
                    {
                        if (value is decimal decimalValue)
                        {
                            jsonWriter.WriteNumber(_document.Columns[cellIndex].Id, decimalValue);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is not a decimal.");
                        }
                    }
                    else if (_document.Columns[cellIndex] is DateTimeColumn)
                    {
                        if (value is DateTimeOffset valueAsDateTimeOffset)
                        {
                            jsonWriter.WriteDateTimeOffset(_document.Columns[cellIndex].Id, valueAsDateTimeOffset);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is not a DateTimeOffset.");
                        }
                    }
                    else if (_document.Columns[cellIndex] is DateOnlyColumn)
                    {
                        if (value is DateOnly valueAsDateOnly)
                        {
                            jsonWriter.WriteDateOnly(_document.Columns[cellIndex].Id, valueAsDateOnly);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is not a DateOnly.");
                        }
                    }
                    else if (_document.Columns[cellIndex] is TimeOnlyColumn)
                    {
                        if (value is TimeOnly valueAsTimeOnly)
                        {
                            jsonWriter.WriteTimeOnly(_document.Columns[cellIndex].Id, valueAsTimeOnly);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is not a TimeOnly.");
                        }
                    }
                    else if (_document.Columns[cellIndex] is EnumColumn)
                    {
                        if (value is string stringValue)
                        {
                            jsonWriter.WriteString(_document.Columns[cellIndex].Id, stringValue);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is not a string.");
                        }
                    }
                    else if (_document.Columns[cellIndex] is EnumSetColumn)
                    {
                        if (value is IEnumerable<string> valueAsArray)
                        {
                            jsonWriter.WritePropertyName(_document.Columns[cellIndex].Id);
                            jsonWriter.WriteStartArray();
                            foreach (var v in valueAsArray)
                            {
                                jsonWriter.WriteStringValue(v);
                            }
                            jsonWriter.WriteEndArray();
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is not a IEnumerable<string>.");
                        }
                    }
                    else if (_document.Columns[cellIndex] is JsonColumn)
                    {
                        jsonWriter.WritePropertyName(_document.Columns[cellIndex].Id);
                        if (value is JsonObject valueAsJsonObject)
                        {
                            JsonSerializer.Serialize(jsonWriter, valueAsJsonObject);
                        }
                        else if (value is JsonArray valueAsJsonArray)
                        {
                            JsonSerializer.Serialize(jsonWriter, valueAsJsonArray);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value is neither a JSON object nor a JSON array.");
                        }
                    }
                }
                else
                {
                    if (_document.Columns[cellIndex].Nullable == true)
                    {
                        jsonWriter.WriteNull(_document.Columns[cellIndex].Id);
                    }
                    else
                    {
                        throw new CodeListParserException($"Null value is not allowed.");
                    }

                }

                cellIndex++;
            }

            jsonWriter.WriteEndObject();
        }

        /// <summary>
        /// Assigns a value to row
        /// </summary>
        /// <param name="column">Column of the value</param>
        /// <param name="value">The value</param>
        /// <exception cref="FormatException">If type of value does not fit to column specification</exception>
        private void AssignValue(Column column, object value)
        {
            if (value is null)
            {
                if (column.Nullable != false)
                {
                    _values[column.Id] = null;
                }
                else
                {
                    throw new FormatException("Value must not be NULL.");
                }
            }
            else
            {
                if (column is StringColumn)
                {
                    _values[column.Id] = Convert.ToString(value);
                }
                else if (column is BooleanColumn)
                {
                    _values[column.Id] = Convert.ToBoolean(value);
                }
                else if (column is IntegerColumn)
                {
                    _values[column.Id] = Convert.ToInt32(value);
                }
                else if (column is NumberColumn)
                {
                    _values[column.Id] = Convert.ToDecimal(value);
                }
                else if (column is EnumColumn)
                {
                    _values[column.Id] = Convert.ToString(value);
                }
                else if (column is EnumSetColumn)
                {
                    if (value is IEnumerable<string>)
                    {
                        _values[column.Id] = value;
                    }
                    else
                    {
                        throw new FormatException("Value is not an IEnumerable<string>");
                    }
                }
                else if (column is DateTimeColumn)
                {
                    if (value is DateTimeOffset)
                    {
                        _values[column.Id] = value;
                    }
                    else
                    {
                        throw new FormatException("Value is not a DateTimeOffset type");
                    }
                }
                else if (column is DateOnlyColumn)
                {
                    if (value is DateOnly)
                    {
                        _values[column.Id] = value;
                    }
                    else
                    {
                        throw new FormatException("Value is not a DateOnly type");
                    }
                }
                else if (column is TimeOnlyColumn)
                {
                    if (value is TimeOnly)
                    {
                        _values[column.Id] = value;
                    }
                    else
                    {
                        throw new FormatException("Value is not a TimeOnly type");
                    }
                }
                else if (column is JsonColumn)
                {
                    if (value is JsonObject || value is JsonArray)
                    {
                        _values[column.Id] = value;
                    }
                    else
                    {
                        throw new FormatException("Value is neither a JsonObject type nor a JsonArray type");
                    }
                }
            }
        }
    }
}