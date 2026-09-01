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
using System.Text.Json.Nodes;

namespace OpenCodeList;

/// <summary>
/// A data row of a code list.
/// </summary>
public class Row : Owned<CodeListDocument>, IEnumerable<KeyValuePair<string, object>>
{
    private readonly Dictionary<string, object> _values = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Row"/> class.
    /// </summary>
    /// <param name="owner">The owner of the data set</param>
    public Row(CodeListDocument owner)
        : base(owner)
    {
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
                if (Owner.Columns.Any(x => x.Id == columnId))
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
            if (Owner.Columns.TryFind(x => x.Id == columnId, out var column))
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
        ArgumentException.ThrowIfNullOrEmpty(columnId);

        return _values.Remove(columnId);
    }

    /// <summary>
    /// Removes the value with reference to a given column
    /// </summary>
    /// <param name="column">The column</param>
    /// <returns>TRUE, if removed</returns>
    public bool RemoveValue(Column column)
    {
        ArgumentNullException.ThrowIfNull(column);

        return RemoveValue(column.Id);
    }

    /// <summary>
    /// Parses a <see cref="JsonElement"/> object into a new <see cref="Row"/> instance.
    /// </summary>
    /// <param name="jsonElement">The json object</param>
    /// <returns>A new <see cref="Row"/> instance</returns>
    internal static Row Parse(CodeListDocument owner, JsonElement jsonElement)
    {
        var row = new Row(owner);

        foreach (var jsonObjectProperty in jsonElement.EnumerateObject())
        {
            if (owner.Columns.TryFind(x => x.Id == jsonObjectProperty.Name, out var column))
            {
                if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Null)
                {
                    if (column.Nullable == true)
                    {
                        row._values[column.Id] = null;
                    }
                    else
                    {
                        throw new CodeListParserException($"Value for column '{column.Id}' must not be NULL.");
                    }
                }
                else
                {
                    if (column is StringColumn)
                    {
                        if (jsonObjectProperty.Value.ValueKind == JsonValueKind.String)
                        {
                            row._values[column.Id] = jsonObjectProperty.Value.GetString();
                        }
                        else if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Object)
                        {
                            row._values[column.Id] = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonObjectProperty.Value);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value for column '{column.Id}' is not a string.");
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
                            throw new CodeListParserException($"Value for column '{column.Id}' is not a boolean.");
                        }
                    }
                    else if (column is IntegerColumn)
                    {
                        if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Number)
                        {
                            row._values[column.Id] = jsonObjectProperty.Value.GetInt64();
                        }
                        else
                        {
                            throw new CodeListParserException($"Value for column '{column.Id}' is not an integer.");
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
                            throw new CodeListParserException($"Value for column '{column.Id}' is not a number.");
                        }
                    }
                    else if (column is DateTimeColumn)
                    {
                        if (jsonObjectProperty.Value.ValueKind == JsonValueKind.String)
                        {
                            row._values[column.Id] = JsonSerializer.Deserialize<DateTimeOffset>(jsonObjectProperty.Value);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value for column '{column.Id}' is not a date-time.");
                        }
                    }
                    else if (column is DateOnlyColumn)
                    {
                        if (jsonObjectProperty.Value.ValueKind == JsonValueKind.String)
                        {
                            row._values[column.Id] = JsonSerializer.Deserialize<DateOnly>(jsonObjectProperty.Value);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value for column '{column.Id}' is not a date.");
                        }
                    }
                    else if (column is TimeOnlyColumn)
                    {
                        if (jsonObjectProperty.Value.ValueKind == JsonValueKind.String)
                        {
                            row._values[column.Id] = JsonSerializer.Deserialize<TimeOnly>(jsonObjectProperty.Value);
                        }
                        else
                        {
                            throw new CodeListParserException($"Value for column '{column.Id}' is not a time.");
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
                            throw new CodeListParserException($"Value for column '{column.Id}' is not an enum.");
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
                                    throw new CodeListParserException($"Value for column '{column.Id}' is not an enum-set.");
                                }
                            }
                            row._values[column.Id] = stringList;
                        }
                        else
                        {
                            throw new CodeListParserException($"Value for column '{column.Id}' is not an array.");
                        }
                    }
                    else if (column is JsonColumn)
                    {
                        if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Object)
                        {
                            row._values[column.Id] = JsonObject.Create(jsonObjectProperty.Value.Clone());
                        }
                        else if (jsonObjectProperty.Value.ValueKind == JsonValueKind.Array)
                        {
                            row._values[column.Id] = JsonArray.Create(jsonObjectProperty.Value.Clone());
                        }
                        else
                        {
                            throw new CodeListParserException($"Value for column '{column.Id}' is not an object or an array.");
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
        jsonWriter.WriteStartObject();

        foreach (var column in Owner.Columns)
        {
            if (_values.TryGetValue(column.Id, out var value))
            {
                if (value is null)
                {
                    if (column.Nullable == true)
                    {
                        jsonWriter.WriteNull(column.Id);
                        continue;
                    }

                    throw new CodeListParserException($"Null value for column '{column.Id}' is not allowed.");
                }

                if (column is StringColumn)
                {
                    if (value is string stringValue)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        jsonWriter.WriteStringValue(stringValue);
                    }
                    else if (value is IDictionary<string, string> dictionaryValue)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        JsonSerializer.Serialize(jsonWriter, dictionaryValue);
                    }
                    else
                    {
                        throw new CodeListParserException("Value is neither a string nor a IDictionary<string, string>.");
                    }
                }
                else if (column is BooleanColumn)
                {
                    if (value is bool boolValue)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        jsonWriter.WriteBooleanValue(boolValue);
                    }
                    else
                    {
                        throw new CodeListParserException("Value is not a bool.");
                    }
                }
                else if (column is IntegerColumn)
                {
                    if (value is long integerValue)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        jsonWriter.WriteNumberValue(integerValue);
                    }
                    else
                    {
                        throw new CodeListParserException("Value is not an int.");
                    }
                }
                else if (column is NumberColumn)
                {
                    if (value is decimal decimalValue)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        jsonWriter.WriteNumberValue(decimalValue);
                    }
                    else
                    {
                        throw new CodeListParserException("Value is not a decimal.");
                    }
                }
                else if (column is DateTimeColumn)
                {
                    if (value is DateTimeOffset valueAsDateTimeOffset)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        JsonSerializer.Serialize(jsonWriter, valueAsDateTimeOffset);
                    }
                    else
                    {
                        throw new CodeListParserException("Value is not a DateTimeOffset.");
                    }
                }
                else if (column is DateOnlyColumn)
                {
                    if (value is DateOnly valueAsDateOnly)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        JsonSerializer.Serialize(jsonWriter, valueAsDateOnly);
                    }
                    else
                    {
                        throw new CodeListParserException("Value is not a DateOnly.");
                    }
                }
                else if (column is TimeOnlyColumn)
                {
                    if (value is TimeOnly valueAsTimeOnly)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        JsonSerializer.Serialize(jsonWriter, valueAsTimeOnly);
                    }
                    else
                    {
                        throw new CodeListParserException("Value is not a TimeOnly.");
                    }
                }
                else if (column is EnumColumn)
                {
                    if (value is string stringValue)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        jsonWriter.WriteStringValue(stringValue);
                    }
                    else
                    {
                        throw new CodeListParserException("Value is not a string.");
                    }
                }
                else if (column is EnumSetColumn)
                {
                    if (value is IEnumerable<string> valueAsArray)
                    {
                        jsonWriter.WritePropertyName(column.Id);
                        jsonWriter.WriteStartArray();
                        foreach (var v in valueAsArray)
                        {
                            jsonWriter.WriteStringValue(v);
                        }
                        jsonWriter.WriteEndArray();
                    }
                    else
                    {
                        throw new CodeListParserException("Value is not an IEnumerable<string>.");
                    }
                }
                else if (column is JsonColumn)
                {
                    jsonWriter.WritePropertyName(column.Id);
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
                        throw new CodeListParserException("Value is neither a JSON object nor a JSON array.");
                    }
                }
            }
            else
            {
                if (column.Optional == true)
                {
                    continue;
                }

                if (column.Nullable == true)
                {
                    jsonWriter.WriteNull(column.Id);
                }
                else
                {
                    throw new CodeListParserException("Null value is not allowed.");
                }
            }
        }

        jsonWriter.WriteEndObject();
    }

    /// <summary>
    /// Assigns a value to row
    /// </summary>
    /// <param name="column">Column of the value</param>
    /// <param name="value">The value</param>
    private void AssignValue(Column column, object value)
    {
        ArgumentNullException.ThrowIfNull(column);

        if (value is null)
        {
            if (column.Nullable == true)
            {
                _values[column.Id] = null;
                return;
            }

            throw new FormatException($"Value for column '{column.Id}' must not be NULL.");
        }

        switch (column)
        {
            case StringColumn:
                {
                    if (value is string)
                    {
                        _values[column.Id] = value;
                    }
                    else if (value is IDictionary<string, string> localizedValue)
                    {
                        _values[column.Id] = localizedValue;
                    }
                    else
                    {
                        throw new FormatException($"Value for column '{column.Id}' must be a string or IDictionary<string, string>.");
                    }

                    break;
                }

            case BooleanColumn:
                {
                    if (value is bool booleanValue)
                    {
                        _values[column.Id] = booleanValue;
                    }
                    else
                    {
                        throw new FormatException($"Value for column '{column.Id}' must be a Boolean.");
                    }

                    break;
                }

            case IntegerColumn:
                {
                    _values[column.Id] = value switch
                    {
                        long v => v,
                        sbyte v => (long)v,
                        byte v => (long)v,
                        short v => (long)v,
                        ushort v => (long)v,
                        int v => (long)v,
                        uint v => (long)v,
                        decimal v when decimal.Truncate(v) == v => (long)v,

                        _ => throw new FormatException($"Value for column '{column.Id}' must be an integer.")
                    };

                    break;
                }

            case NumberColumn:
                {
                    _values[column.Id] = value switch
                    {
                        decimal v => v,
                        sbyte v => (decimal)v,
                        byte v => (decimal)v,
                        short v => (decimal)v,
                        ushort v => (decimal)v,
                        int v => (decimal)v,
                        uint v => (decimal)v,
                        long v => (decimal)v,
                        ulong v => (decimal)v,

                        _ => throw new FormatException($"Value for column '{column.Id}' must be a number.")
                    };

                    break;
                }

            case EnumColumn:
                {
                    if (value is string enumValue)
                    {
                        _values[column.Id] = enumValue;
                    }
                    else
                    {
                        throw new FormatException($"Value for column '{column.Id}' must be a string.");
                    }

                    break;
                }

            case EnumSetColumn:
                {
                    if (value is IEnumerable<string> enumValues)
                    {
                        _values[column.Id] = enumValues.ToArray();
                    }
                    else
                    {
                        throw new FormatException($"Value for column '{column.Id}' must be an IEnumerable<string>.");
                    }

                    break;
                }

            case DateTimeColumn:
                {
                    if (value is DateTimeOffset dateTimeValue)
                    {
                        _values[column.Id] = dateTimeValue;
                    }
                    else
                    {
                        throw new FormatException($"Value for column '{column.Id}' must be a DateTimeOffset.");
                    }

                    break;
                }

            case DateOnlyColumn:
                {
                    if (value is DateOnly dateValue)
                    {
                        _values[column.Id] = dateValue;
                    }
                    else
                    {
                        throw new FormatException($"Value for column '{column.Id}' must be a DateOnly.");
                    }

                    break;
                }

            case TimeOnlyColumn:
                {
                    if (value is TimeOnly timeValue)
                    {
                        _values[column.Id] = timeValue;
                    }
                    else
                    {
                        throw new FormatException($"Value for column '{column.Id}' must be a TimeOnly.");
                    }

                    break;
                }

            case JsonColumn:
                {
                    if (value is JsonObject or JsonArray)
                    {
                        _values[column.Id] = value;
                    }
                    else
                    {
                        throw new FormatException($"Value for column '{column.Id}' must be a JsonObject or JsonArray.");
                    }

                    break;
                }

            default:
                throw new NotSupportedException($"Column type '{column.GetType().Name}' is not supported.");
        }
    }
}