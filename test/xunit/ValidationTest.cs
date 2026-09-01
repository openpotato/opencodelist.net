#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*
 *    OpenCodeList.NET
 *
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// This class contains unit tests for validating code list documents.
/// </summary>
public class ValidationTest
{
    [Fact]
    public void Validate_Allows_Annotation_With_AppInfo_Only()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Annotation = new Annotation
        {
            AppInfo = new JsonObject { ["key"] = "value" }
        };

        document.Validate();
    }

    [Fact]
    public void Validate_Allows_Empty_Meta_CodeListSet()
    {
        var document = new CodeListSetDocument();
        document.Identification.ShortName = "Set";
        document.Identification.CanonicalUri = new Uri("urn:test:set");
        document.Identification.CanonicalVersionUri = new Uri("urn:test:set:1");
        document.ClearContent(true);

        document.Validate();
        Assert.True(document.MetaOnly);
    }

    [Fact]
    public void Validate_Allows_Optional_Identification_Collections_To_Be_Null()
    {
        var document = TestDocumentFactory.CreateValidCodeList();

        Assert.Null(document.Identification.Tags);
        Assert.Null(document.Identification.ChangeLog);
        Assert.Null(document.Identification.LocationUrls);
        Assert.Null(document.Identification.AlternateLanguageLocations);
        Assert.Null(document.Identification.AlternateFormatLocations);

        document.Validate();
    }

    [Fact]
    public void Validate_Applies_String_Length_And_Pattern_To_Localized_Values()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
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
            ["en"] = "abc"
        };

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Minimal_CodeList_Succeeds()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Validate();
    }

    [Fact]
    public void Validate_Rejects_CodeList_Without_Key()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Keys.Clear();

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Duplicate_Column_Ids()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        var duplicate = document.Columns.Add<StringColumn>();
        duplicate.Id = "code";
        duplicate.Name = NonLocalizedString.Create("Duplicate");

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Duplicate_EnumSet_Value()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        var column = document.Columns.Add<EnumSetColumn>();
        column.Id = "flags";
        column.Name = NonLocalizedString.Create("Flags");
        column.Optional = false;
        column.Nullable = false;
        column.Members.Add(new EnumMember { Value = "a" });

        var row = document.Rows.Add();
        row["code"] = "A";
        row["flags"] = new List<string> { "a", "a" };

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Duplicate_Key_Values()
    {
        var document = TestDocumentFactory.CreateValidCodeList();

        var row1 = document.Rows.Add();
        row1["code"] = "A";
        var row2 = document.Rows.Add();
        row2["code"] = "A";

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Empty_NonMeta_CodeListSet()
    {
        var document = new CodeListSetDocument();
        document.Identification.ShortName = "Set";
        document.Identification.CanonicalUri = new Uri("urn:test:set");
        document.Identification.CanonicalVersionUri = new Uri("urn:test:set:1");

        Assert.False(document.MetaOnly);
        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Invalid_Alternate_Format_Location()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Identification.AlternateFormatLocations =
        [
            new MimeTypedUri
            {
                MimeType = "",
                Url = new Uri("https://example.com/list.csv")
            }
        ];

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Invalid_Bcp47_Tag_In_Localized_Metadata()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Columns["code"].Name = new LocalizedString(new Dictionary<string, string>
        {
            ["not_a_language_tag"] = "Code"
        });

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Invalid_Bcp47_Tag_In_Localized_Row_Value()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
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

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Invalid_Number_Bounds()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        var column = document.Columns.Add<NumberColumn>();
        column.Id = "value";
        column.Name = NonLocalizedString.Create("Value");
        column.MinValue = 10;
        column.MaxValue = 5;
        column.Optional = true;

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Inverted_Validity_Range()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Identification.ValidFrom = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero);
        document.Identification.ValidTo = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Key_Column_That_Is_Nullable()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Columns["code"].Nullable = true;

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Key_Column_That_Is_Optional()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Columns["code"].Optional = true;

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void Validate_Rejects_Publisher_Extension_Without_X_Prefix()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Identification.Publisher = new Publisher
        {
            ShortName = "Publisher",
            Extensions = new Dictionary<string, JsonElement>
            {
                ["contact"] = JsonSerializer.SerializeToElement("info@example.com")
            }
        };

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }
    
    [Fact]
    public void Validate_Rejects_Undefined_Enum_Value()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        var column = document.Columns.Add<EnumColumn>();
        column.Id = "status";
        column.Name = NonLocalizedString.Create("Status");
        column.Optional = false;
        column.Nullable = false;
        column.Members.Add(new EnumMember { Value = "active" });

        var row = document.Rows.Add();
        row["code"] = "A";
        row["status"] = "unknown";

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }
}
