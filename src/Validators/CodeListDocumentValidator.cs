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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace OpenCodeList;

/// <summary>
/// An abstract OpenCodeList base class for <see cref="CodeListDocument"/> and
/// <see cref="CodeListSetDocument"/> 
/// </summary>
public sealed class CodeListDocumentValidator : CodeListBaseValidator
{
    private readonly CodeListDocument _document;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeListDocumentValidator"/> class.
    /// </summary>
    public CodeListDocumentValidator(CodeListDocument document)
        : base(document)
    {
        _document = document;
    }

    /// <summary>
    /// Validates the <see cref="CodeListDocument"/> instance.
    /// </summary>
    public override void Validate()
    {
        base.Validate();

        ValidateColumns();
        ValidateKeys();
        ValidateForeignKeys();
        ValidateRows();
        ValidateKeyValuesUnique();

        if (_document.MetaOnly && _document.Rows.Count > 0)
        {
            throw new CodeListValidatorException($"Meta document must not contain '{PropertyNames.DataSet}.{PropertyNames.Rows}'.");
        }
    }

    /// <summary>
    /// Validates the enum members in the specified list of <see cref="EnumMember"/> instances.
    /// </summary>
    private static void ValidateEnumMembers(IList<EnumMember> members, string columnPath)
    {
        if (members is null || members.Count == 0)
        {
            throw new CodeListValidatorException($"'{columnPath}.{PropertyNames.Members}' must not be empty.");
        }

        var values = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < members.Count; i++)
        {
            var member = members[i];
            var path = $"{columnPath}.{PropertyNames.Members}[{i}]";

            if (member is null)
            {
                throw new CodeListValidatorException($"'{path}' must not be null.");
            }

            ValidateRequiredString(member.Value, $"{path}.{PropertyNames.Value}");
            ValidateLocalizableString(member.Description, $"{path}.{PropertyNames.Description}", false);

            if (!values.Add(member.Value))
            {
                throw new CodeListValidatorException($"Enum member value '{member.Value}' must be unique in '{columnPath}.{PropertyNames.Members}'.");
            }
        }
    }

    /// <summary>
    /// Validates the bounds of a <see cref="NumberColumn"/> instance.
    /// </summary>
    private static void ValidateNumberBounds(NumberColumn numberColumn, string path)
    {
        if (numberColumn.MinValue is not null && numberColumn.MaxValue is not null && numberColumn.MinValue > numberColumn.MaxValue)
        {
            throw new CodeListValidatorException($"'{path}.{PropertyNames.MinValue}' must be less than or equal to '{path}.{PropertyNames.MaxValue}'.");
        }

        if (numberColumn.ExclusiveMinValue is not null && numberColumn.MaxValue is not null && numberColumn.ExclusiveMinValue >= numberColumn.MaxValue)
        {
            throw new CodeListValidatorException($"'{path}.{PropertyNames.ExclusiveMinValue}' must be less than '{path}.{PropertyNames.MaxValue}'.");
        }

        if (numberColumn.MinValue is not null && numberColumn.ExclusiveMaxValue is not null && numberColumn.MinValue >= numberColumn.ExclusiveMaxValue)
        {
            throw new CodeListValidatorException($"'{path}.{PropertyNames.MinValue}' must be less than '{path}.{PropertyNames.ExclusiveMaxValue}'.");
        }

        if (numberColumn.ExclusiveMinValue is not null && numberColumn.ExclusiveMaxValue is not null && numberColumn.ExclusiveMinValue >= numberColumn.ExclusiveMaxValue)
        {
            throw new CodeListValidatorException($"'{path}.{PropertyNames.ExclusiveMinValue}' must be less than '{path}.{PropertyNames.ExclusiveMaxValue}'.");
        }
    }

    /// <summary>
    /// Validates the value of a specific column in a row of the <see cref="CodeListDocument"/> instance.
    /// </summary>
    private static void ValidateRowValue(Column column, object value, int rowIndex)
    {
        var path = $"{PropertyNames.DataSet}.{PropertyNames.Rows}[{rowIndex}].{column.Id}";

        if (value is null)
        {
            if (column.Nullable == true)
            {
                return;
            }

            throw new CodeListValidatorException($"'{path}' must not be null.");
        }

        switch (column)
        {
            case StringColumn stringColumn:
                ValidateStringColumnValue(stringColumn, value, path);
                break;

            case BooleanColumn:
                if (value is not bool)
                {
                    throw new CodeListValidatorException($"'{path}' must be a boolean value.");
                }
                break;

            case IntegerColumn integerColumn:
                if (value is not long integerValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be an integer value.");
                }

                if (integerColumn.MinValue is not null && integerValue < integerColumn.MinValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be greater than or equal to {integerColumn.MinValue}.");
                }

                if (integerColumn.MaxValue is not null && integerValue > integerColumn.MaxValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be less than or equal to {integerColumn.MaxValue}.");
                }
                break;

            case NumberColumn numberColumn:
                if (value is not decimal decimalValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be a decimal value.");
                }

                if (numberColumn.MinValue is not null && decimalValue < numberColumn.MinValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be greater than or equal to {numberColumn.MinValue}.");
                }

                if (numberColumn.ExclusiveMinValue is not null && decimalValue <= numberColumn.ExclusiveMinValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be greater than {numberColumn.ExclusiveMinValue}.");
                }

                if (numberColumn.MaxValue is not null && decimalValue > numberColumn.MaxValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be less than or equal to {numberColumn.MaxValue}.");
                }

                if (numberColumn.ExclusiveMaxValue is not null && decimalValue >= numberColumn.ExclusiveMaxValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be less than {numberColumn.ExclusiveMaxValue}.");
                }
                break;

            case DateTimeColumn dateTimeColumn:
                if (value is not DateTimeOffset dateTimeValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be a date-time value.");
                }

                if (dateTimeColumn.MinValue is not null && dateTimeValue < dateTimeColumn.MinValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be greater than or equal to {dateTimeColumn.MinValue}.");
                }

                if (dateTimeColumn.MaxValue is not null && dateTimeValue > dateTimeColumn.MaxValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be less than or equal to {dateTimeColumn.MaxValue}.");
                }
                break;

            case DateOnlyColumn dateOnlyColumn:
                if (value is not DateOnly dateOnlyValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be a date value.");
                }

                if (dateOnlyColumn.MinValue is not null && dateOnlyValue < dateOnlyColumn.MinValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be greater than or equal to {dateOnlyColumn.MinValue}.");
                }

                if (dateOnlyColumn.MaxValue is not null && dateOnlyValue > dateOnlyColumn.MaxValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be less than or equal to {dateOnlyColumn.MaxValue}.");
                }
                break;

            case TimeOnlyColumn timeOnlyColumn:
                if (value is not TimeOnly timeOnlyValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be a time value.");
                }

                if (timeOnlyColumn.MinValue is not null && timeOnlyValue < timeOnlyColumn.MinValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be greater than or equal to {timeOnlyColumn.MinValue}.");
                }

                if (timeOnlyColumn.MaxValue is not null && timeOnlyValue > timeOnlyColumn.MaxValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be less than or equal to {timeOnlyColumn.MaxValue}.");
                }
                break;

            case EnumColumn enumColumn:
                if (value is not string enumValue)
                {
                    throw new CodeListValidatorException($"'{path}' must be a string value.");
                }

                if (!enumColumn.Members.Any(m => m.Value == enumValue))
                {
                    throw new CodeListValidatorException($"'{path}' contains undefined enum value '{enumValue}'.");
                }
                break;

            case EnumSetColumn enumSetColumn:
                if (value is not IEnumerable<string> enumValues)
                {
                    throw new CodeListValidatorException($"'{path}' must be an array of strings.");
                }

                var allowedValues = new HashSet<string>(enumSetColumn.Members.Select(m => m.Value), StringComparer.Ordinal);
                var usedValues = new HashSet<string>(StringComparer.Ordinal);

                foreach (var enumSetValue in enumValues)
                {
                    ValidateRequiredString(enumSetValue, path, false);

                    if (!usedValues.Add(enumSetValue))
                    {
                        throw new CodeListValidatorException($"'{path}' must not contain duplicate enum values.");
                    }

                    if (!allowedValues.Contains(enumSetValue))
                    {
                        throw new CodeListValidatorException($"'{path}' contains undefined enum value '{enumSetValue}'.");
                    }
                }
                break;

            case JsonColumn:
                if (value is not JsonObject && value is not JsonArray)
                {
                    throw new CodeListValidatorException($"'{path}' must be a JSON object or JSON array.");
                }
                break;

            default:
                throw new CodeListValidatorException($"'{path}' uses an unsupported column type.");
        }
    }

    /// <summary>
    /// Validates the value of a <see cref="StringColumn"/> instance, which can be either a string or a localized object.
    /// </summary>
    private static void ValidateStringColumnValue(StringColumn stringColumn, object value, string path)
    {
        switch (value)
        {
            case string stringValue:
                ValidateStringValueAgainstStringColumn(stringColumn, stringValue, path);
                break;

            case IDictionary<string, string> localizedValue:
                if (localizedValue.Count == 0)
                {
                    throw new CodeListValidatorException($"'{path}' must contain at least one localized value.");
                }

                foreach (var localizedEntry in localizedValue)
                {
                    ValidateLanguageTag(localizedEntry.Key, $"{path}[{localizedEntry.Key}]", true);
                    ValidateStringValueAgainstStringColumn(stringColumn, localizedEntry.Value, $"{path}[{localizedEntry.Key}]");
                }
                break;

            default:
                throw new CodeListValidatorException($"'{path}' must be a string or localized object.");
        }
    }

    /// <summary>
    /// Validates the minimum and maximum length of a <see cref="StringColumn"/> instance.
    /// </summary>
    private static void ValidateStringLengthRange(int? minLength, int? maxLength, string path)
    {
        if (minLength is not null && minLength < 0)
        {
            throw new CodeListValidatorException($"'{path}.{PropertyNames.MinLength}' must be greater than or equal to 0.");
        }

        if (maxLength is not null && maxLength < 0)
        {
            throw new CodeListValidatorException($"'{path}.{PropertyNames.MaxLength}' must be greater than or equal to 0.");
        }

        if (minLength is not null && maxLength is not null && minLength > maxLength)
        {
            throw new CodeListValidatorException($"'{path}.{PropertyNames.MinLength}' must be less than or equal to '{path}.{PropertyNames.MaxLength}'.");
        }
    }

    /// <summary>
    /// Validates a string value against the constraints of a <see cref="StringColumn"/> instance, including minimum and maximum length and pattern matching.
    /// </summary>
    private static void ValidateStringValueAgainstStringColumn(StringColumn column, string value, string path)
    {
        ValidateRequiredString(value, path, false);

        if (column.MinLength is not null && value.Length < column.MinLength)
        {
            throw new CodeListValidatorException($"'{path}' must have a minimum length of {column.MinLength}.");
        }

        if (column.MaxLength is not null && value.Length > column.MaxLength)
        {
            throw new CodeListValidatorException($"'{path}' must have a maximum length of {column.MaxLength}.");
        }

        if (!string.IsNullOrWhiteSpace(column.Pattern) && !Regex.IsMatch(value, column.Pattern, RegexOptions.CultureInvariant))
        {
            throw new CodeListValidatorException($"'{path}' does not match pattern '{column.Pattern}'.");
        }
    }

    /// <summary>
    /// Validates the columns in the <see cref="CodeListDocument"/> instance.
    /// </summary>
    private void ValidateColumns()
    {
        if (_document.Columns.Count == 0)
        {
            throw new CodeListValidatorException($"'{PropertyNames.ColumnSet}.{PropertyNames.Columns}' must contain at least one column.");
        }

        var ids = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < _document.Columns.Count; i++)
        {
            var column = _document.Columns[i];
            var path = $"{PropertyNames.ColumnSet}.{PropertyNames.Columns}[{i}]";

            if (column is null)
            {
                throw new CodeListValidatorException($"'{path}' must not be null.");
            }

            ValidateRequiredString(column.Id, $"{path}.{PropertyNames.Id}");
            if (!ids.Add(column.Id))
            {
                throw new CodeListValidatorException($"Column ID '{column.Id}' must be unique.");
            }

            ValidateLocalizableString(column.Name, $"{path}.{PropertyNames.Name}", true);
            ValidateLocalizableString(column.Description, $"{path}.{PropertyNames.Description}", false);

            switch (column)
            {
                case StringColumn stringColumn:
                    ValidateLanguageTag(stringColumn.Language, $"{path}.{PropertyNames.Language}", false);
                    ValidateStringLengthRange(stringColumn.MinLength, stringColumn.MaxLength, path);
                    break;

                case EnumColumn enumColumn:
                    ValidateLanguageTag(enumColumn.Language, $"{path}.{PropertyNames.Language}", false);
                    ValidateEnumMembers(enumColumn.Members, path);
                    break;

                case EnumSetColumn enumSetColumn:
                    ValidateLanguageTag(enumSetColumn.Language, $"{path}.{PropertyNames.Language}", false);
                    ValidateEnumMembers(enumSetColumn.Members, path);
                    break;

                case IntegerColumn integerColumn:
                    if (integerColumn.MinValue is not null && integerColumn.MaxValue is not null && integerColumn.MinValue > integerColumn.MaxValue)
                    {
                        throw new CodeListValidatorException($"'{path}.{PropertyNames.MinValue}' must be less than or equal to '{path}.{PropertyNames.MaxValue}'.");
                    }
                    break;

                case NumberColumn numberColumn:
                    ValidateNumberBounds(numberColumn, path);
                    break;

                case DateOnlyColumn dateOnlyColumn:
                    if (dateOnlyColumn.MinValue is not null && dateOnlyColumn.MaxValue is not null && dateOnlyColumn.MinValue > dateOnlyColumn.MaxValue)
                    {
                        throw new CodeListValidatorException($"'{path}.{PropertyNames.MinValue}' must be less than or equal to '{path}.{PropertyNames.MaxValue}'.");
                    }
                    break;

                case DateTimeColumn dateTimeColumn:
                    if (dateTimeColumn.MinValue is not null && dateTimeColumn.MaxValue is not null && dateTimeColumn.MinValue > dateTimeColumn.MaxValue)
                    {
                        throw new CodeListValidatorException($"'{path}.{PropertyNames.MinValue}' must be less than or equal to '{path}.{PropertyNames.MaxValue}'.");
                    }
                    break;

                case TimeOnlyColumn timeOnlyColumn:
                    if (timeOnlyColumn.MinValue is not null && timeOnlyColumn.MaxValue is not null && timeOnlyColumn.MinValue > timeOnlyColumn.MaxValue)
                    {
                        throw new CodeListValidatorException($"'{path}.{PropertyNames.MinValue}' must be less than or equal to '{path}.{PropertyNames.MaxValue}'.");
                    }
                    break;

                case BooleanColumn:
                case JsonColumn:
                    break;

                default:
                    throw new CodeListValidatorException($"'{path}' contains an unsupported column type.");
            }
        }
    }
    /// <summary>
    /// Validates the foreign keys in the <see cref="CodeListDocument"/> instance.
    /// </summary>
    private void ValidateForeignKeys()
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < _document.ForeignKeys.Count; i++)
        {
            var foreignKey = _document.ForeignKeys[i];
            var path = $"{PropertyNames.ColumnSet}.{PropertyNames.ForeignKeys}[{i}]";

            if (foreignKey is null)
            {
                throw new CodeListValidatorException($"'{path}' must not be null.");
            }

            ValidateRequiredString(foreignKey.Id, $"{path}.{PropertyNames.Id}");
            if (!ids.Add(foreignKey.Id))
            {
                throw new CodeListValidatorException($"Foreign key ID '{foreignKey.Id}' must be unique.");
            }

            ValidateLocalizableString(foreignKey.Name, $"{path}.{PropertyNames.Name}", false);
            ValidateLocalizableString(foreignKey.Description, $"{path}.{PropertyNames.Description}", false);

            if (foreignKey.Columns.Count == 0)
            {
                throw new CodeListValidatorException($"'{path}.{PropertyNames.ColumnIds}' must not be empty.");
            }

            if (foreignKey.KeyRef is null)
            {
                throw new CodeListValidatorException($"'{path}.{PropertyNames.KeyRef}' must not be null.");
            }

            ValidateRequiredString(foreignKey.KeyRef.KeyId, $"{path}.{PropertyNames.KeyRef}.{PropertyNames.KeyId}");
            ValidateExternalCodeListRef(foreignKey.KeyRef.CodeListRef, $"{path}.{PropertyNames.KeyRef}.{PropertyNames.CodeListRef}");
        }
    }

    /// <summary>
    /// Validates the keys in the <see cref="CodeListDocument"/> instance.
    /// </summary>
    private void ValidateKeys()
    {
        if (_document.Keys.Count == 0)
        {
            throw new CodeListValidatorException($"'{PropertyNames.ColumnSet}.{PropertyNames.Keys}' must contain at least one key.");
        }

        var ids = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < _document.Keys.Count; i++)
        {
            var key = _document.Keys[i];
            var path = $"{PropertyNames.ColumnSet}.{PropertyNames.Keys}[{i}]";

            if (key is null)
            {
                throw new CodeListValidatorException($"'{path}' must not be null.");
            }

            ValidateRequiredString(key.Id, $"{path}.{PropertyNames.Id}");
            if (!ids.Add(key.Id))
            {
                throw new CodeListValidatorException($"Key ID '{key.Id}' must be unique.");
            }

            ValidateLocalizableString(key.Name, $"{path}.{PropertyNames.Name}", false);
            ValidateLocalizableString(key.Description, $"{path}.{PropertyNames.Description}", false);

            if (key.Columns.Count == 0)
            {
                throw new CodeListValidatorException($"'{path}.{PropertyNames.ColumnIds}' must not be empty.");
            }

            var keyColumnIds = new HashSet<string>(StringComparer.Ordinal);
            for (var j = 0; j < key.Columns.Count; j++)
            {
                var column = key.Columns[j] ?? throw new CodeListValidatorException($"'{path}.{PropertyNames.ColumnIds}[{j}]' must not be null.");
                if (!keyColumnIds.Add(column.Id))
                {
                    throw new CodeListValidatorException($"'{path}.{PropertyNames.ColumnIds}' must not contain duplicate column IDs.");
                }

                if (column.Optional == true)
                {
                    throw new CodeListValidatorException($"Column '{column.Id}' is part of key '{key.Id}' and therefore must not be optional.");
                }

                if (column.Nullable == true)
                {
                    throw new CodeListValidatorException($"Column '{column.Id}' is part of key '{key.Id}' and therefore must not be nullable.");
                }
            }
        }

        if (_document.DefaultKey is not null && !_document.Keys.Contains(x => ReferenceEquals(x, _document.DefaultKey)))
        {
            throw new CodeListValidatorException($"'{PropertyNames.ColumnSet}.{PropertyNames.DefaultKey}' references a key that does not belong to this document.");
        }
    }

    /// <summary>
    /// Validates that the key values in the <see cref="CodeListDocument"/> instance are unique across all rows.
    /// </summary>
    private void ValidateKeyValuesUnique()
    {
        for (var keyIndex = 0; keyIndex < _document.Keys.Count; keyIndex++)
        {
            var key = _document.Keys[keyIndex];
            var values = new HashSet<string>(StringComparer.Ordinal);

            for (var rowIndex = 0; rowIndex < _document.Rows.Count; rowIndex++)
            {
                var row = _document.Rows[rowIndex];
                var keyParts = new string[key.Columns.Count];

                for (var columnIndex = 0; columnIndex < key.Columns.Count; columnIndex++)
                {
                    var column = key.Columns[columnIndex];
                    var keyValue = row[column.Id];
                    keyParts[columnIndex] = keyValue?.ToString() ?? string.Empty;
                }

                var compoundKey = string.Join("\u001F", keyParts);
                if (!values.Add(compoundKey))
                {
                    throw new CodeListValidatorException($"Duplicate key value detected for key '{key.Id}' in row {rowIndex}.");
                }
            }
        }
    }
    /// <summary>
    /// Validates the rows in the <see cref="CodeListDocument"/> instance.
    /// </summary>
    private void ValidateRows()
    {
        for (var rowIndex = 0; rowIndex < _document.Rows.Count; rowIndex++)
        {
            var row = _document.Rows[rowIndex] ?? throw new CodeListValidatorException($"'{PropertyNames.DataSet}.{PropertyNames.Rows}[{rowIndex}]' must not be null.");
            var presentColumns = new HashSet<string>(StringComparer.Ordinal);

            foreach (var valuePair in row)
            {
                var columnId = valuePair.Key;
                if (!presentColumns.Add(columnId))
                {
                    throw new CodeListValidatorException($"Duplicate column '{columnId}' in row {rowIndex}.");
                }

                if (!_document.Columns.TryFind(x => x.Id == columnId, out var column))
                {
                    throw new CodeListValidatorException($"Column with ID '{columnId}' not found for row {rowIndex}.");
                }

                ValidateRowValue(column, valuePair.Value, rowIndex);
            }

            for (var columnIndex = 0; columnIndex < _document.Columns.Count; columnIndex++)
            {
                var column = _document.Columns[columnIndex];
                if (column.Optional == true)
                {
                    continue;
                }

                if (!presentColumns.Contains(column.Id))
                {
                    throw new CodeListValidatorException($"Required column '{column.Id}' is missing in row {rowIndex}.");
                }
            }
        }
    }
}