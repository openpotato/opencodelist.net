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

using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Tests for serialization and row handling in the library.
/// </summary>
public class SerializationAndRowTests
{
    [Fact]
    public void Annotation_AppInfoOnly_Roundtrips()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.Annotation = new Annotation
        {
            AppInfo = new JsonObject
            {
                ["application"] = "test"
            }
        };

        new CodeListDocumentValidator().TestValidate(document).ShouldNotHaveAnyValidationErrors();

        using var stream = new MemoryStream();
        document.Save(stream, false);
        stream.Position = 0;

        var loaded = CodeListDocument.Load(stream);
        new CodeListDocumentValidator().TestValidate(loaded).ShouldNotHaveAnyValidationErrors();

        Assert.NotNull(loaded.Annotation.AppInfo);
        Assert.Null(loaded.Annotation.Descriptions);
        Assert.Equal("test", loaded.Annotation.AppInfo["application"]!.GetValue<string>());
    }

    [Fact]
    public void Clear_MetaOnlyDocument_PreservesMetaOnlyState()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.ClearContent(true);
        Assert.True(document.MetaOnly);

        document.Clear();

        Assert.True(document.MetaOnly);
    }

    [Fact]
    public void LocalizedRowValue_Roundtrip_PreservesAllLanguages()
    {
        var document = TestDocumentFactory.CreateCodeList();

        var nameColumn = document.Columns.Add<StringColumn>();
        nameColumn.Id = "name";
        nameColumn.Name = new LocalizedString(new Dictionary<string, string>
        {
            ["en"] = "Name",
            ["de"] = "Name"
        });
        nameColumn.Optional = false;
        nameColumn.Nullable = false;

        var row = document.Rows.Add();
        row["code"] = "A";
        row["name"] = new Dictionary<string, string>
        {
            ["en"] = "Germany",
            ["de"] = "Deutschland"
        };

        using var stream = new MemoryStream();
        document.Save(stream, false);
        stream.Position = 0;

        var loaded = CodeListDocument.Load(stream);
        new CodeListDocumentValidator().TestValidate(loaded).ShouldNotHaveAnyValidationErrors();

        var value = Assert.IsType<Dictionary<string, string>>(loaded.Rows[0]["name"]);
        Assert.Equal("Germany", value["en"]);
        Assert.Equal("Deutschland", value["de"]);
    }

    [Fact]
    public void RowAssignment_NullableColumn_AllowsNull()
    {
        var document = TestDocumentFactory.CreateCodeList();

        var column = document.Columns.Add<StringColumn>();
        column.Id = "text";
        column.Name = NonLocalizedString.Create("Text");
        column.Optional = true;
        column.Nullable = true;

        var row = document.Rows.Add();
        row["code"] = "A";
        row["text"] = null;

        new CodeListDocumentValidator().TestValidate(document).ShouldNotHaveAnyValidationErrors();
        Assert.Null(row["text"]);
    }

    [Fact]
    public void RowAssignment_NonNullableColumn_RejectsNull()
    {
        var document = TestDocumentFactory.CreateCodeList();

        var column = document.Columns.Add<StringColumn>();
        column.Id = "text";
        column.Name = NonLocalizedString.Create("Text");
        column.Optional = true;

        var row = document.Rows.Add();
        row["code"] = "A";

        Assert.Throws<FormatException>(() => row["text"] = null);
    }

    [Fact]
    public void PublisherExtensions_Roundtrip_PreservesValues()
    {
        var document = TestDocumentFactory.CreateCodeList();
        document.Identification.Publisher = new Publisher
        {
            ShortName = "Publisher",
            Extensions = new Dictionary<string, JsonElement>
            {
                ["x-contact"] = JsonSerializer.SerializeToElement("info@example.com")
            }
        };

        using var stream = new MemoryStream();
        document.Save(stream, false);
        stream.Position = 0;

        var loaded = CodeListDocument.Load(stream);

        Assert.Equal("info@example.com", loaded.Identification.Publisher.Extensions["x-contact"].GetString());
    }

    [Fact]
    public void Save_MinimalDocument_OmitsOptionalNullsAndWritesEmptyRows()
    {
        var document = TestDocumentFactory.CreateCodeList();

        using var stream = new MemoryStream();
        document.Save(stream, false);
        stream.Position = 0;

        using var json = JsonDocument.Parse(stream);
        var codeList = json.RootElement.GetProperty("codeList");
        var identification = codeList.GetProperty("identification");

        Assert.False(identification.TryGetProperty("tags", out _));
        Assert.False(identification.TryGetProperty("changeLog", out _));
        Assert.False(identification.TryGetProperty("locationUrls", out _));
        Assert.False(identification.TryGetProperty("alternateLanguageLocations", out _));
        Assert.False(identification.TryGetProperty("alternateFormatLocations", out _));

        var rows = codeList.GetProperty("dataSet").GetProperty("rows");
        Assert.Equal(JsonValueKind.Array, rows.ValueKind);
        Assert.Equal(0, rows.GetArrayLength());
    }

    [Fact]
    public void Save_MissingOptionalRowValue_OmitsProperty()
    {
        var document = TestDocumentFactory.CreateCodeList();

        var optionalColumn = document.Columns.Add<StringColumn>();
        optionalColumn.Id = "optionalText";
        optionalColumn.Name = NonLocalizedString.Create("Optional text");
        optionalColumn.Optional = true;
        optionalColumn.Nullable = false;

        var row = document.Rows.Add();
        row["code"] = "A";

        new CodeListDocumentValidator().TestValidate(document).ShouldNotHaveAnyValidationErrors();

        using var stream = new MemoryStream();
        document.Save(stream, false);
        stream.Position = 0;

        using var json = JsonDocument.Parse(stream);
        var jsonRow = json.RootElement
            .GetProperty("codeList")
            .GetProperty("dataSet")
            .GetProperty("rows")[0];

        Assert.True(jsonRow.TryGetProperty("code", out _));
        Assert.False(jsonRow.TryGetProperty("optionalText", out _));
    }
}
