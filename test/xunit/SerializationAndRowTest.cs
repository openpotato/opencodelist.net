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
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Tests for serialization and row handling in the OpenCodeList.NET library.
/// </summary>
public class SerializationAndRowTest
{
    [Fact]
    public void Annotation_With_AppInfo_Only_Roundtrips()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.Annotation = new Annotation
        {
            AppInfo = new JsonObject
            {
                ["application"] = "test"
            }
        };

        document.Validate();

        using var stream = new MemoryStream();
        document.Save(stream, false);
        stream.Position = 0;

        var loaded = CodeListDocument.Load(stream);
        loaded.Validate();

        Assert.NotNull(loaded.Annotation.AppInfo);
        Assert.Null(loaded.Annotation.Descriptions);
        Assert.Equal("test", loaded.Annotation.AppInfo["application"]!.GetValue<string>());
    }

    [Fact]
    public void Clear_Preserves_MetaOnly_State()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        document.ClearContent(true);
        Assert.True(document.MetaOnly);

        document.Clear();

        Assert.True(document.MetaOnly);
    }

    [Fact]
    public void Localized_Row_Value_Roundtrip_Preserves_All_Languages()
    {
        var document = TestDocumentFactory.CreateValidCodeList();

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
        loaded.Validate();

        var value = Assert.IsType<Dictionary<string, string>>(loaded.Rows[0]["name"]);
        Assert.Equal("Germany", value["en"]);
        Assert.Equal("Deutschland", value["de"]);
    }

    [Fact]
    public void Row_Assignment_Allows_Null_When_Nullable_Is_True()
    {
        var document = TestDocumentFactory.CreateValidCodeList();

        var column = document.Columns.Add<StringColumn>();
        column.Id = "text";
        column.Name = NonLocalizedString.Create("Text");
        column.Optional = true;
        column.Nullable = true;

        var row = document.Rows.Add();
        row["code"] = "A";
        row["text"] = null;

        document.Validate();
        Assert.Null(row["text"]);
    }

    [Fact]
    public void Row_Assignment_Rejects_Null_When_Nullable_Is_Omitted()
    {
        var document = TestDocumentFactory.CreateValidCodeList();

        var column = document.Columns.Add<StringColumn>();
        column.Id = "text";
        column.Name = NonLocalizedString.Create("Text");
        column.Optional = true;

        var row = document.Rows.Add();
        row["code"] = "A";

        Assert.Throws<FormatException>(() => row["text"] = null);
    }

    [Fact]
    public void Save_And_Load_Preserves_Publisher_Extensions()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
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
    public void Save_Minimal_Document_Omits_Optional_Null_Properties_And_Writes_Empty_Rows()
    {
        var document = TestDocumentFactory.CreateValidCodeList();

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
    public void Save_Omits_Missing_Optional_Row_Value()
    {
        var document = TestDocumentFactory.CreateValidCodeList();

        var optionalColumn = document.Columns.Add<StringColumn>();
        optionalColumn.Id = "optionalText";
        optionalColumn.Name = NonLocalizedString.Create("Optional text");
        optionalColumn.Optional = true;
        optionalColumn.Nullable = false;

        var row = document.Rows.Add();
        row["code"] = "A";

        document.Validate();

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
