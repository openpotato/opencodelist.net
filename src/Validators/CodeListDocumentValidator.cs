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

using FluentValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace OpenCodeList;

/// <summary>
/// FluentValidation validator for <see cref="CodeListDocument"/> instances.
/// </summary>
public sealed class CodeListDocumentValidator : CodeListBaseValidator<CodeListDocument>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeListDocumentValidator"/> class.
    /// </summary>
    public CodeListDocumentValidator()
    {
        RuleFor(document => document.Columns)
            .Must(columns => columns.Count > 0)
            .WithMessage("At least one column must be defined.")
            .OverridePropertyName($"{PropertyNames.ColumnSet}.{PropertyNames.Columns}");

        RuleForEach(document => document.Columns)
            .SetValidator(new ColumnValidator())
            .OverridePropertyName($"{PropertyNames.ColumnSet}.{PropertyNames.Columns}");

        RuleForEach(document => document.Keys)
            .SetValidator(new KeyValidator())
            .OverridePropertyName($"{PropertyNames.ColumnSet}.{PropertyNames.Keys}");

        RuleForEach(document => document.ForeignKeys)
            .SetValidator(new ForeignKeyValidator())
            .OverridePropertyName($"{PropertyNames.ColumnSet}.{PropertyNames.ForeignKeys}");

        RuleFor(document => document.Columns)
            .Custom(ValidateUniqueColumnIds);

        RuleFor(document => document.Keys)
            .Custom(ValidateUniqueKeyIds);

        RuleFor(document => document.ForeignKeys)
            .Custom(ValidateUniqueForeignKeyIds);

        RuleFor(document => document)
            .Custom(ValidateDefaultKey);

        RuleFor(document => document)
            .Custom(ValidateRows);

        RuleFor(document => document)
            .Custom(ValidateUniqueKeyValues);

        RuleFor(document => document)
            .Must(document => !document.MetaOnly || document.Rows.Count == 0)
            .WithMessage($"Meta document must not contain '{PropertyNames.DataSet}.{PropertyNames.Rows}'.");
    }

    /// <summary>
    /// Validates that the default key of the document references a key that belongs to the document.
    /// </summary>
    private static void ValidateDefaultKey(CodeListDocument document, ValidationContext<CodeListDocument> context)
    {
        if (document.DefaultKey is not null && !document.Keys.Contains(key => ReferenceEquals(key, document.DefaultKey)))
        {
            context.AddFailure(
                $"{PropertyNames.ColumnSet}.{PropertyNames.DefaultKey}",
                "Default key must reference a key that belongs to this document.");
        }
    }

    /// <summary>
    /// Validates that the value of an enum-set column is an array of strings, each of which is a valid member of the enum set.
    /// </summary>
    private static void ValidateEnumSetValue(EnumSetColumn column, object value, string path, ValidationContext<CodeListDocument> context)
    {
        if (value is not IEnumerable<string> enumValues)
        {
            context.AddFailure(path, "Value must be an array of strings.");
            return;
        }

        var allowedValues = new HashSet<string>(
            column.Members.Where(member => member is not null).Select(member => member.Value),
            StringComparer.Ordinal);
        var usedValues = new HashSet<string>(StringComparer.Ordinal);

        foreach (var enumValue in enumValues)
        {
            if (enumValue is null)
            {
                context.AddFailure(path, "Enum-set value must not be null.");
                continue;
            }

            if (!usedValues.Add(enumValue))
            {
                context.AddFailure(path, $"Enum-set value '{enumValue}' must not occur more than once.");
            }

            if (!allowedValues.Contains(enumValue))
            {
                context.AddFailure(path, $"Undefined enum value '{enumValue}'.");
            }
        }
    }

    /// <summary>
    /// Validates the rows of the document.
    /// </summary>
    private static void ValidateRows(CodeListDocument document, ValidationContext<CodeListDocument> context)
    {
        for (var rowIndex = 0; rowIndex < document.Rows.Count; rowIndex++)
        {
            var row = document.Rows[rowIndex];
            var rowPath = $"{PropertyNames.DataSet}.{PropertyNames.Rows}[{rowIndex}]";

            if (row is null)
            {
                context.AddFailure(rowPath, "Row must not be null.");
                continue;
            }

            var presentColumns = new HashSet<string>(StringComparer.Ordinal);

            foreach (var valuePair in row)
            {
                var columnId = valuePair.Key;
                var valuePath = $"{rowPath}.{columnId}";

                if (!presentColumns.Add(columnId))
                {
                    context.AddFailure(valuePath, $"Column '{columnId}' must not occur more than once in a row.");
                    continue;
                }

                if (!document.Columns.TryFind(column => column.Id == columnId, out var column))
                {
                    context.AddFailure(valuePath, $"Column with ID '{columnId}' is not defined.");
                    continue;
                }

                ValidateRowValue(column, valuePair.Value, valuePath, context);
            }

            foreach (var column in document.Columns)
            {
                if (column.Optional == true)
                {
                    continue;
                }

                if (!presentColumns.Contains(column.Id))
                {
                    context.AddFailure($"{rowPath}.{column.Id}", $"Required column '{column.Id}' is missing.");
                }
            }
        }
    }

    /// <summary>
    /// Validates the value of a row for a specific column, checking for type correctness and constraints defined by the column.
    /// </summary>
    private static void ValidateRowValue(Column column, object value, string path, ValidationContext<CodeListDocument> context)
    {
        if (value is null)
        {
            if (column.Nullable != true)
            {
                context.AddFailure(path, "Value must not be null.");
            }
            return;
        }

        switch (column)
        {
            case StringColumn stringColumn:
                ValidateStringColumnValue(stringColumn, value, path, context);
                break;

            case BooleanColumn:
                if (value is not bool)
                {
                    context.AddFailure(path, "Value must be a boolean.");
                }
                break;

            case IntegerColumn integerColumn:
                if (value is not long integerValue)
                {
                    context.AddFailure(path, "Value must be an integer.");
                    break;
                }

                if (integerColumn.MinValue is not null && integerValue < integerColumn.MinValue)
                {
                    context.AddFailure(path, $"Value must be greater than or equal to {integerColumn.MinValue}.");
                }

                if (integerColumn.MaxValue is not null && integerValue > integerColumn.MaxValue)
                {
                    context.AddFailure(path, $"Value must be less than or equal to {integerColumn.MaxValue}.");
                }
                break;

            case NumberColumn numberColumn:
                if (value is not decimal decimalValue)
                {
                    context.AddFailure(path, "Value must be a decimal number.");
                    break;
                }

                if (numberColumn.MinValue is not null && decimalValue < numberColumn.MinValue)
                {
                    context.AddFailure(path, $"Value must be greater than or equal to {numberColumn.MinValue}.");
                }

                if (numberColumn.ExclusiveMinValue is not null && decimalValue <= numberColumn.ExclusiveMinValue)
                {
                    context.AddFailure(path, $"Value must be greater than {numberColumn.ExclusiveMinValue}.");
                }

                if (numberColumn.MaxValue is not null && decimalValue > numberColumn.MaxValue)
                {
                    context.AddFailure(path, $"Value must be less than or equal to {numberColumn.MaxValue}.");
                }

                if (numberColumn.ExclusiveMaxValue is not null && decimalValue >= numberColumn.ExclusiveMaxValue)
                {
                    context.AddFailure(path, $"Value must be less than {numberColumn.ExclusiveMaxValue}.");
                }
                break;

            case DateTimeColumn dateTimeColumn:
                if (value is not DateTimeOffset dateTimeValue)
                {
                    context.AddFailure(path, "Value must be a date-time.");
                    break;
                }

                if (dateTimeColumn.MinValue is not null && dateTimeValue < dateTimeColumn.MinValue)
                {
                    context.AddFailure(path, $"Value must be greater than or equal to {dateTimeColumn.MinValue}.");
                }

                if (dateTimeColumn.MaxValue is not null && dateTimeValue > dateTimeColumn.MaxValue)
                {
                    context.AddFailure(path, $"Value must be less than or equal to {dateTimeColumn.MaxValue}.");
                }
                break;

            case DateOnlyColumn dateOnlyColumn:
                if (value is not DateOnly dateOnlyValue)
                {
                    context.AddFailure(path, "Value must be a date.");
                    break;
                }

                if (dateOnlyColumn.MinValue is not null && dateOnlyValue < dateOnlyColumn.MinValue)
                {
                    context.AddFailure(path, $"Value must be greater than or equal to {dateOnlyColumn.MinValue}.");
                }

                if (dateOnlyColumn.MaxValue is not null && dateOnlyValue > dateOnlyColumn.MaxValue)
                {
                    context.AddFailure(path, $"Value must be less than or equal to {dateOnlyColumn.MaxValue}.");
                }
                break;

            case TimeOnlyColumn timeOnlyColumn:
                if (value is not TimeOnly timeOnlyValue)
                {
                    context.AddFailure(path, "Value must be a time.");
                    break;
                }

                if (timeOnlyColumn.MinValue is not null && timeOnlyValue < timeOnlyColumn.MinValue)
                {
                    context.AddFailure(path, $"Value must be greater than or equal to {timeOnlyColumn.MinValue}.");
                }

                if (timeOnlyColumn.MaxValue is not null && timeOnlyValue > timeOnlyColumn.MaxValue)
                {
                    context.AddFailure(path, $"Value must be less than or equal to {timeOnlyColumn.MaxValue}.");
                }
                break;

            case EnumColumn enumColumn:
                if (value is not string enumValue)
                {
                    context.AddFailure(path, "Value must be a string.");
                }
                else if (!enumColumn.Members.Any(member => member?.Value == enumValue))
                {
                    context.AddFailure(path, $"Undefined enum value '{enumValue}'.");
                }
                break;

            case EnumSetColumn enumSetColumn:
                ValidateEnumSetValue(enumSetColumn, value, path, context);
                break;

            case JsonColumn:
                if (value is not JsonObject && value is not JsonArray)
                {
                    context.AddFailure(path, "Value must be a JSON object or JSON array.");
                }
                break;

            default:
                context.AddFailure(path, "Unsupported column type.");
                break;
        }
    }

    /// <summary>
    /// Validates the value of a string column, which can be either a single string or a localized string object.
    /// </summary>
    private static void ValidateStringColumnValue(StringColumn column, object value, string path, ValidationContext<CodeListDocument> context)
    {
        switch (value)
        {
            case string stringValue:
                ValidateStringValue(column, stringValue, path, context);
                break;

            case IDictionary<string, string> localizedValue:
                if (localizedValue.Count == 0)
                {
                    context.AddFailure(path, "At least one localized value must be specified.");
                    break;
                }

                foreach (var entry in localizedValue)
                {
                    var localizedPath = $"{path}[{entry.Key}]";

                    if (!ValidatorHelpers.IsLanguageTag(entry.Key))
                    {
                        context.AddFailure(localizedPath, "Localized value key must be a valid BCP 47 language tag.");
                    }

                    ValidateStringValue(column, entry.Value, localizedPath, context);
                }
                break;

            default:
                context.AddFailure(path, "Value must be a string or localized string object.");
                break;
        }
    }

    /// <summary>
    /// Validates a single string value against the constraints defined in a <see cref="StringColumn"/>, such as minimum/maximum length and pattern matching.
    /// </summary>
    private static void ValidateStringValue(StringColumn column, string value, string path, ValidationContext<CodeListDocument> context)
    {
        if (value is null)
        {
            context.AddFailure(path, "Value must not be null.");
            return;
        }

        if (column.MinLength is not null && value.Length < column.MinLength)
        {
            context.AddFailure(path, $"Value must have a minimum length of {column.MinLength}.");
        }

        if (column.MaxLength is not null && value.Length > column.MaxLength)
        {
            context.AddFailure(path, $"Value must have a maximum length of {column.MaxLength}.");
        }

        if (!string.IsNullOrWhiteSpace(column.Pattern))
        {
            try
            {
                if (!Regex.IsMatch(value, column.Pattern, RegexOptions.CultureInvariant))
                {
                    context.AddFailure(path, $"Value does not match pattern '{column.Pattern}'.");
                }
            }
            catch (ArgumentException)
            {
                // Invalid patterns are reported by ColumnValidator. Avoid throwing from validation.
            }
        }
    }

    /// <summary>
    /// Validates that all column IDs in the document are unique.
    /// </summary>
    private static void ValidateUniqueColumnIds(Columns columns, ValidationContext<CodeListDocument> context)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var index = 0;

        foreach (var column in columns)
        {
            if (column is not null && !string.IsNullOrWhiteSpace(column.Id) && !ids.Add(column.Id))
            {
                context.AddFailure($"{PropertyNames.ColumnSet}.{PropertyNames.Columns}[{index}].{PropertyNames.Id}", $"Column ID '{column.Id}' must be unique.");
            }
            index++;
        }
    }

    /// <summary>
    /// Validates that all foreign key IDs in the document are unique.
    /// </summary>
    private static void ValidateUniqueForeignKeyIds(ForeignKeys foreignKeys, ValidationContext<CodeListDocument> context)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var index = 0;

        foreach (var foreignKey in foreignKeys)
        {
            if (foreignKey is not null && !string.IsNullOrWhiteSpace(foreignKey.Id) && !ids.Add(foreignKey.Id))
            {
                context.AddFailure($"{PropertyNames.ColumnSet}.{PropertyNames.ForeignKeys}[{index}].{PropertyNames.Id}", $"Foreign key ID '{foreignKey.Id}' must be unique.");
            }
            index++;
        }
    }

    /// <summary>
    /// Validates that all key IDs in the document are unique.
    /// </summary>
    private static void ValidateUniqueKeyIds(Keys keys, ValidationContext<CodeListDocument> context)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var index = 0;

        foreach (var key in keys)
        {
            if (key is not null && !string.IsNullOrWhiteSpace(key.Id) && !ids.Add(key.Id))
            {
                context.AddFailure($"{PropertyNames.ColumnSet}.{PropertyNames.Keys}[{index}].{PropertyNames.Id}", $"Key ID '{key.Id}' must be unique.");
            }
            index++;
        }
    }

    /// <summary>
    /// Validates that the values of the keys in the document are unique across all rows, ensuring that no two rows have the same combination of key values for any defined key.
    /// </summary>
    private static void ValidateUniqueKeyValues(CodeListDocument document, ValidationContext<CodeListDocument> context)
    {
        foreach (var key in document.Keys)
        {
            if (key is null || key.Columns.Count == 0)
            {
                continue;
            }

            var values = new HashSet<string>(StringComparer.Ordinal);

            for (var rowIndex = 0; rowIndex < document.Rows.Count; rowIndex++)
            {
                var row = document.Rows[rowIndex];
                if (row is null)
                {
                    continue;
                }

                var keyParts = new string[key.Columns.Count];
                var complete = true;

                for (var columnIndex = 0; columnIndex < key.Columns.Count; columnIndex++)
                {
                    var column = key.Columns[columnIndex];
                    if (column is null)
                    {
                        complete = false;
                        break;
                    }

                    object keyValue;
                    try
                    {
                        keyValue = row[column.Id];
                    }
                    catch (ArgumentException)
                    {
                        complete = false;
                        break;
                    }

                    if (keyValue is null)
                    {
                        complete = false;
                        break;
                    }

                    keyParts[columnIndex] = Convert.ToString(keyValue, CultureInfo.InvariantCulture) ?? string.Empty;
                }

                if (!complete)
                {
                    continue;
                }

                var compoundKey = string.Join("\u001F", keyParts);
                if (!values.Add(compoundKey))
                {
                    context.AddFailure(
                        $"{PropertyNames.DataSet}.{PropertyNames.Rows}[{rowIndex}]",
                        $"Duplicate key value for key '{key.Id}'.");
                }
            }
        }
    }
}

/// <summary>
/// Validator for <see cref="Column"/> instances
/// </summary>
internal sealed class ColumnValidator : AbstractValidator<Column>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnValidator"/> class.
    /// </summary>
    public ColumnValidator()
    {
        RuleFor(column => column.Id)
            .NotEmpty()
            .WithMessage("Column ID must not be empty.")
            .OverridePropertyName(PropertyNames.Id);

        RuleFor(column => column.Name)
            .Custom((value, context) => ValidatorHelpers.ValidateLocalizableString(context, PropertyNames.Name, value));

        RuleFor(column => column.Description)
            .Custom((value, context) => ValidatorHelpers.ValidateOptionalLocalizableString(context, PropertyNames.Description, value));

        RuleFor(column => column)
            .Custom(ValidateColumnType);
    }

    /// <summary>
    /// Validates the specific type of a column, checking for constraints and properties that are unique to each column type.
    /// </summary>
    private static void ValidateColumnType(Column column, ValidationContext<Column> context)
    {
        switch (column)
        {
            case StringColumn stringColumn:
                ValidateStringColumn(stringColumn, context);
                break;

            case EnumColumn enumColumn:
                ValidateEnumColumn(enumColumn.Language, enumColumn.Members, context);
                break;

            case EnumSetColumn enumSetColumn:
                ValidateEnumColumn(enumSetColumn.Language, enumSetColumn.Members, context);
                break;

            case IntegerColumn integerColumn:
                if (integerColumn.MinValue is not null && integerColumn.MaxValue is not null && integerColumn.MinValue > integerColumn.MaxValue)
                {
                    context.AddFailure(PropertyNames.MinValue, "Minimum value must be less than or equal to maximum value.");
                }
                break;

            case NumberColumn numberColumn:
                ValidateNumberColumn(numberColumn, context);
                break;

            case DateOnlyColumn dateColumn:
                if (dateColumn.MinValue is not null && dateColumn.MaxValue is not null && dateColumn.MinValue > dateColumn.MaxValue)
                {
                    context.AddFailure(PropertyNames.MinValue, "Minimum value must be less than or equal to maximum value.");
                }
                break;

            case DateTimeColumn dateTimeColumn:
                if (dateTimeColumn.MinValue is not null && dateTimeColumn.MaxValue is not null && dateTimeColumn.MinValue > dateTimeColumn.MaxValue)
                {
                    context.AddFailure(PropertyNames.MinValue, "Minimum value must be less than or equal to maximum value.");
                }
                break;

            case TimeOnlyColumn timeColumn:
                if (timeColumn.MinValue is not null && timeColumn.MaxValue is not null && timeColumn.MinValue > timeColumn.MaxValue)
                {
                    context.AddFailure(PropertyNames.MinValue, "Minimum value must be less than or equal to maximum value.");
                }
                break;

            case JsonColumn jsonColumn:
                if (jsonColumn.SchemaUri is not null && !jsonColumn.SchemaUri.IsAbsoluteUri)
                {
                    context.AddFailure(PropertyNames.SchemaUri, "Schema URI must be absolute.");
                }
                break;

            case BooleanColumn:
                break;

            default:
                context.AddFailure("Unsupported column type.");
                break;
        }
    }

    /// <summary>
    /// Validates the properties of an enum or enum-set column.
    /// </summary>
    private static void ValidateEnumColumn(string language, IList<EnumMember> members, ValidationContext<Column> context)
    {
        if (!ValidatorHelpers.IsOptionalLanguageTag(language))
        {
            context.AddFailure(PropertyNames.Language, "Language must contain a valid BCP 47 language tag.");
        }

        if (members is null || members.Count == 0)
        {
            context.AddFailure(PropertyNames.Members, "At least one enum member must be defined.");
            return;
        }

        var values = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < members.Count; i++)
        {
            var member = members[i];
            if (member is null)
            {
                context.AddFailure($"{PropertyNames.Members}[{i}]", "Enum member must not be null.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(member.Value))
            {
                context.AddFailure($"{PropertyNames.Members}[{i}].{PropertyNames.Value}", "Enum member value must not be empty.");
            }
            else if (!values.Add(member.Value))
            {
                context.AddFailure($"{PropertyNames.Members}[{i}].{PropertyNames.Value}", $"Enum member value '{member.Value}' must be unique.");
            }

            if (member.Description is not null)
            {
                switch (member.Description)
                {
                    case NonLocalizedString nonLocalized when string.IsNullOrWhiteSpace(nonLocalized.Value):
                        context.AddFailure($"{PropertyNames.Members}[{i}].{PropertyNames.Description}", "Description must not be empty.");
                        break;
                    case LocalizedString localized:
                        if (localized.Values.Count == 0)
                        {
                            context.AddFailure($"{PropertyNames.Members}[{i}].{PropertyNames.Description}", "At least one localized description must be specified.");
                        }
                        foreach (var entry in localized.Values)
                        {
                            if (!ValidatorHelpers.IsLanguageTag(entry.Key))
                            {
                                context.AddFailure($"{PropertyNames.Members}[{i}].{PropertyNames.Description}[{entry.Key}]", "Localized description key must be a valid BCP 47 language tag.");
                            }
                            if (entry.Value is null)
                            {
                                context.AddFailure($"{PropertyNames.Members}[{i}].{PropertyNames.Description}[{entry.Key}]", "Localized description must not be null.");
                            }
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Validates the properties of a number column.
    /// </summary>
    private static void ValidateNumberColumn(NumberColumn column, ValidationContext<Column> context)
    {
        if (column.MinValue is not null && column.MaxValue is not null && column.MinValue > column.MaxValue)
        {
            context.AddFailure(PropertyNames.MinValue, "Minimum value must be less than or equal to maximum value.");
        }

        if (column.ExclusiveMinValue is not null && column.MaxValue is not null && column.ExclusiveMinValue >= column.MaxValue)
        {
            context.AddFailure(PropertyNames.ExclusiveMinValue, "Exclusive minimum value must be less than maximum value.");
        }

        if (column.MinValue is not null && column.ExclusiveMaxValue is not null && column.MinValue >= column.ExclusiveMaxValue)
        {
            context.AddFailure(PropertyNames.MinValue, "Minimum value must be less than exclusive maximum value.");
        }

        if (column.ExclusiveMinValue is not null && column.ExclusiveMaxValue is not null && column.ExclusiveMinValue >= column.ExclusiveMaxValue)
        {
            context.AddFailure(PropertyNames.ExclusiveMinValue, "Exclusive minimum value must be less than exclusive maximum value.");
        }
    }

    /// <summary>
    /// Validates the properties of a string column.
    /// </summary>
    private static void ValidateStringColumn(StringColumn column, ValidationContext<Column> context)
    {
        if (!ValidatorHelpers.IsOptionalLanguageTag(column.Language))
        {
            context.AddFailure(PropertyNames.Language, "Language must contain a valid BCP 47 language tag.");
        }

        if (column.MinLength is not null && column.MinLength < 0)
        {
            context.AddFailure(PropertyNames.MinLength, "Minimum length must be greater than or equal to 0.");
        }

        if (column.MaxLength is not null && column.MaxLength < 0)
        {
            context.AddFailure(PropertyNames.MaxLength, "Maximum length must be greater than or equal to 0.");
        }

        if (column.MinLength is not null && column.MaxLength is not null && column.MinLength > column.MaxLength)
        {
            context.AddFailure(PropertyNames.MinLength, "Minimum length must be less than or equal to maximum length.");
        }

        if (!string.IsNullOrWhiteSpace(column.Pattern))
        {
            try
            {
                _ = new Regex(column.Pattern, RegexOptions.CultureInvariant);
            }
            catch (ArgumentException)
            {
                context.AddFailure(PropertyNames.Pattern, "Pattern must contain a valid regular expression.");
            }
        }
    }
}

/// <summary>
/// Validator for <see cref="ForeignKey"/> instances
/// </summary>
internal sealed class ForeignKeyValidator : AbstractValidator<ForeignKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyValidator"/> class.
    /// </summary>
    public ForeignKeyValidator()
    {
        RuleFor(foreignKey => foreignKey.Id)
            .NotEmpty()
            .WithMessage("Foreign key ID must not be empty.")
            .OverridePropertyName(PropertyNames.Id);

        RuleFor(foreignKey => foreignKey.Name)
            .Custom((value, context) => ValidatorHelpers.ValidateLocalizableString(context, PropertyNames.Name, value));

        RuleFor(foreignKey => foreignKey.Description)
            .Custom((value, context) => ValidatorHelpers.ValidateOptionalLocalizableString(context, PropertyNames.Description, value));

        RuleFor(foreignKey => foreignKey.Columns)
            .Must(columns => columns is not null && columns.Count > 0)
            .WithMessage("Foreign key must reference at least one column.")
            .OverridePropertyName(PropertyNames.ColumnIds);

        RuleFor(foreignKey => foreignKey.KeyRef)
            .NotNull()
            .WithMessage("Key reference must not be null.")
            .SetValidator(new ExternalKeyRefValidator())
            .OverridePropertyName(PropertyNames.KeyRef);
    }
}

/// <summary>
/// Validator for <see cref="Key"/> instances
/// </summary>
internal sealed class KeyValidator : AbstractValidator<Key>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyValidator"/> class.
    /// </summary>
    public KeyValidator()
    {
        RuleFor(key => key.Id)
            .NotEmpty()
            .WithMessage("Key ID must not be empty.")
            .OverridePropertyName(PropertyNames.Id);

        RuleFor(key => key.Name)
            .Custom((value, context) => ValidatorHelpers.ValidateOptionalLocalizableString(context, PropertyNames.Name, value));

        RuleFor(key => key.Description)
            .Custom((value, context) => ValidatorHelpers.ValidateOptionalLocalizableString(context, PropertyNames.Description, value));

        RuleFor(key => key.Columns)
            .Must(columns => columns is not null && columns.Count > 0)
            .WithMessage("Key must reference at least one column.")
            .OverridePropertyName(PropertyNames.ColumnIds);

        RuleFor(key => key.Columns)
            .Custom(ValidateColumns);
    }

    /// <summary>
    /// Validates that the columns referenced by a key are valid, ensuring that they are not null, not optional, and not nullable, and that there are no duplicate column IDs.
    /// </summary>
    private static void ValidateColumns(ColumnRefs columns, ValidationContext<Key> context)
    {
        if (columns is null)
        {
            return;
        }

        var ids = new HashSet<string>(StringComparer.Ordinal);
        var index = 0;

        foreach (var column in columns)
        {
            if (column is null)
            {
                context.AddFailure($"{PropertyNames.ColumnIds}[{index}]", "Referenced column must not be null.");
                index++;
                continue;
            }

            if (!ids.Add(column.Id))
            {
                context.AddFailure($"{PropertyNames.ColumnIds}[{index}]", $"Column ID '{column.Id}' must not occur more than once in a key.");
            }

            if (column.Optional == true)
            {
                context.AddFailure($"{PropertyNames.ColumnIds}[{index}]", $"Key column '{column.Id}' must not be optional.");
            }

            if (column.Nullable == true)
            {
                context.AddFailure($"{PropertyNames.ColumnIds}[{index}]", $"Key column '{column.Id}' must not be nullable.");
            }

            index++;
        }
    }
}

/// <summary>
/// Validator for <see cref="ExternalKeyRef"/> instances
/// </summary>
internal sealed class ExternalKeyRefValidator : AbstractValidator<ExternalKeyRef>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalKeyRefValidator"/> class.
    /// </summary>
    public ExternalKeyRefValidator()
    {
        RuleFor(reference => reference.KeyId)
            .NotEmpty()
            .WithMessage("Key ID must not be empty.")
            .OverridePropertyName(PropertyNames.KeyId);

        RuleFor(reference => reference.CodeListRef)
            .NotNull()
            .WithMessage("Code-list reference must not be null.")
            .SetValidator(new ExternalCodeListBaseRefValidator())
            .OverridePropertyName(PropertyNames.CodeListRef);
    }
}
