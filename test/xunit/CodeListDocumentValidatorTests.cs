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
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for <see cref="CodeListDocumentValidator"/>.
/// </summary>
public class CodeListDocumentValidatorTests
{
    private readonly CodeListDocumentValidator _validator = new();

    [Fact]
    public void Annotation_AppInfoOnly_IsValid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.Annotation = new Annotation
        {
            AppInfo = new JsonObject { ["key"] = "value" }
        };

        _validator.TestValidate(document)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DuplicateColumnIds_AreInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var duplicate = document.Columns.Add<StringColumn>();
        duplicate.Id = "code";
        duplicate.Name = NonLocalizedString.Create("Duplicate");

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("columnSet.columns[1].id");
    }

    [Fact]
    public void DuplicateEnumSetValue_IsInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var column = document.Columns.Add<EnumSetColumn>();
        column.Id = "flags";
        column.Name = NonLocalizedString.Create("Flags");
        column.Optional = false;
        column.Nullable = false;
        column.Members.Add(new EnumMember { Value = "a" });

        var row = document.Rows.Add();
        row["code"] = "A";
        row["flags"] = new List<string> { "a", "a" };

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("dataSet.rows[0].flags");
    }

    [Fact]
    public void DuplicateKeyValues_AreInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();

        var row1 = document.Rows.Add();
        row1["code"] = "A";
        var row2 = document.Rows.Add();
        row2["code"] = "A";

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("dataSet.rows[1]");
    }

    [Fact]
    public void EmptyRows_IsValid()
    {
        using var stream = TestDocumentFactory.CreateCodeListAsStream();
        var document = CodeListDocument.Load(stream);

        Assert.False(document.MetaOnly);
        Assert.Empty(document.Rows);

        _validator.TestValidate(document)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InvalidAlternateFormatLocation_IsInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.Identification.AlternateFormatLocations =
        [
            new MimeTypedUri
            {
                MimeType = "",
                Url = new Uri("https://example.com/list.csv")
            }
        ];

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("identification.alternateFormatLocations[0].mimeType");
    }

    [Fact]
    public void InvalidBcp47Tag_LocalizedMetadata_IsInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.Columns["code"].Name = new LocalizedString(new Dictionary<string, string>
        {
            ["not_a_language_tag"] = "Code"
        });

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("columnSet.columns[0].name");
    }

    [Fact]
    public void InvalidBcp47Tag_LocalizedRowValue_IsInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var column = document.Columns.Add<StringColumn>();
        column.Id = "name";
        column.Name = NonLocalizedString.Create("Name");
        column.Optional = false;
        column.Nullable = false;

        var row = document.Rows.Add();
        row["code"] = "A";
        row["name"] = new Dictionary<string, string>
        {
            ["not_a_language_tag"] = "Name"
        };

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("dataSet.rows[0].name[not_a_language_tag]");
    }

    [Fact]
    public void InvalidRegularExpression_IsValidationFailure()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var column = document.Columns.Add<StringColumn>();
        column.Id = "value";
        column.Name = NonLocalizedString.Create("Value");
        column.Pattern = "[";
        column.Optional = true;

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("columnSet.columns[1].pattern");
    }

    [Fact]
    public void InvertedNumberBounds_AreInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var column = document.Columns.Add<NumberColumn>();
        column.Id = "value";
        column.Name = NonLocalizedString.Create("Value");
        column.MinValue = 10;
        column.MaxValue = 5;
        column.Optional = true;

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("columnSet.columns[1].minValue");
    }

    [Fact]
    public void InvertedValidityRange_IsInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.Identification.ValidFrom = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero);
        document.Identification.ValidTo = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("identification.validFrom");
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void KeyColumn_NullableOrOptional_IsInvalid(bool nullable, bool optional)
    {
        var document = TestDocumentFactory.CreateCodeList();
        var column = document.Columns["code"];
        column.Nullable = nullable;
        column.Optional = optional;

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("columnSet.keys[0].columnIds[0]");
    }

    [Fact]
    public void LocalizedStringValue_LengthAndPatternConstraints_AreApplied()
    {
        var document = TestDocumentFactory.CreateCodeList();

        var column = document.Columns.Add<StringColumn>();
        column.Id = "label";
        column.Name = NonLocalizedString.Create("Label");
        column.MinLength = 2;
        column.MaxLength = 4;
        column.Pattern = "^[A-Z]+$";
        column.Optional = false;
        column.Nullable = false;

        var row = document.Rows.Add();
        row["code"] = "A";
        row["label"] = new Dictionary<string, string>
        {
            ["en"] = "abc",  
            ["de"] = "ABCDE" 
        };

        var result = _validator.TestValidate(document);

        result.ShouldHaveValidationErrorFor("dataSet.rows[0].label[en]");
        result.ShouldHaveValidationErrorFor("dataSet.rows[0].label[de]");
    }

    [Fact]
    public void MinimalCodeList_IsValid()
    {
        var document = TestDocumentFactory.CreateCodeList();

        _validator.TestValidate(document)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void MissingRequiredRowValue_IsInvalid()
    {
        using var stream = TestDocumentFactory.CreateCodeListAsStream(row: "{}");
        var document = CodeListDocument.Load(stream);

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("dataSet.rows[0].code");
    }

    [Fact]
    public void NullableKeyColumn_ReportsKeyColumnPath()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.Columns["code"].Nullable = true;

        var result = _validator.TestValidate(document);

        result.ShouldHaveValidationErrorFor("columnSet.keys[0].columnIds[0]");
        Assert.Contains(result.Errors, error =>
            error.PropertyName.Contains("columnSet.keys", StringComparison.Ordinal));
    }

    [Fact]
    public void OptionalIdentificationCollections_Null_IsValid()
    {
        var document = TestDocumentFactory.CreateCodeList();

        Assert.Null(document.Identification.Tags);
        Assert.Null(document.Identification.ChangeLog);
        Assert.Null(document.Identification.LocationUrls);
        Assert.Null(document.Identification.AlternateLanguageLocations);
        Assert.Null(document.Identification.AlternateFormatLocations);
        _validator.TestValidate(document).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void PublisherExtension_WithoutXPrefix_IsInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.Identification.Publisher = new Publisher
        {
            ShortName = "Publisher",
            Extensions = new Dictionary<string, JsonElement>
            {
                ["contact"] = JsonSerializer.SerializeToElement("info@example.com")
            }
        };

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("identification.publisher.contact");
    }

    [Fact]
    public void UndefinedEnumValue_IsInvalid()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var column = document.Columns.Add<EnumColumn>();
        column.Id = "status";
        column.Name = NonLocalizedString.Create("Status");
        column.Optional = false;
        column.Nullable = false;
        column.Members.Add(new EnumMember { Value = "active" });

        var row = document.Rows.Add();
        row["code"] = "A";
        row["status"] = "unknown";

        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("dataSet.rows[0].status");
    }

    [Fact]
    public void ValidateAndThrow_InvalidDocument_ThrowsValidationException()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.Keys.Add();

        Assert.Throws<ValidationException>(() => _validator.ValidateAndThrow(document));
    }
}
