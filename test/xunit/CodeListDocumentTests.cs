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
using System.Threading.Tasks;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for <see cref="CodeListDocument"/>.
/// </summary>
public class CodeListDocumentTests
{

    [Fact]
    public async Task ConstructAndRoundtrip_ByCode_PreservesDocument()
    {
        var document = new CodeListDocument();

        document.Identification.ShortName = "ConstructedCodeList";
        document.Identification.CanonicalUri = new Uri("https://example.com/codelist");
        document.Identification.CanonicalVersionUri = new Uri("https://example.com/codelist/1.0.0");

        var codeColumn = document.Columns.Add<StringColumn>();
        codeColumn.Id = "code";
        codeColumn.Name = NonLocalizedString.Create("Code");

        var sortOrderColumn = document.Columns.Add<IntegerColumn>();
        sortOrderColumn.Id = "sortOrder";
        sortOrderColumn.Name = NonLocalizedString.Create("Sort Order");

        var key = document.Keys.Add();
        key.Id = "codeKey";
        key.Name = NonLocalizedString.Create("Code Key");
        key.Columns.Add(codeColumn);

        document.DefaultKey = key;

        var row = document.Rows.Add();
        row["code"] = "A";
        row["sortOrder"] = 1;

        Assert.False(document.MetaOnly);
        Assert.Equal(2, document.Columns.Count);
        Assert.Equal(1, document.Keys.Count);
        Assert.Equal("codeKey", document.DefaultKey.Id);
        Assert.Equal(1, document.Rows.Count);
        Assert.Equal("A", document.Rows[0]["code"] as string);
        Assert.Equal(1, document.Rows[0]["sortOrder"] as long?);

        await using var stream = new MemoryStream();

        await document.SaveAsync(stream, TestContext.Current.CancellationToken);

        stream.Position = 0;

        var loadedDocument = await CodeListDocument.LoadAsync(stream, TestContext.Current.CancellationToken);

        Assert.Equal("ConstructedCodeList", loadedDocument.Identification.ShortName);
        Assert.Equal(new Uri("https://example.com/codelist"), loadedDocument.Identification.CanonicalUri);
        Assert.Equal(new Uri("https://example.com/codelist/1.0.0"), loadedDocument.Identification.CanonicalVersionUri);
        Assert.Equal(2, loadedDocument.Columns.Count);
        Assert.Equal(1, loadedDocument.Keys.Count);
        Assert.Equal("codeKey", loadedDocument.DefaultKey.Id);
        Assert.Equal(1, loadedDocument.Rows.Count);
        Assert.Equal("A", loadedDocument.Rows[0]["code"] as string);
        Assert.Equal(1, loadedDocument.Rows[0]["sortOrder"] as long?);
    }

    [Fact]
    public async Task InMemoryRoundtrip_ByCode_PreservesContent()
    {
        var originalDocument = new CodeListDocument();

        originalDocument.Comments.Add("Created in memory");
        originalDocument.Identification.ShortName = "InMemoryCodeList";
        originalDocument.Identification.LongName = "In memory code list";
        originalDocument.Identification.CanonicalUri = new Uri("https://example.com/inmemory");
        originalDocument.Identification.CanonicalVersionUri = new Uri("https://example.com/inmemory/1.0.0");

        var codeColumn = originalDocument.Columns.Add<StringColumn>();
        codeColumn.Id = "code";
        codeColumn.Name = NonLocalizedString.Create("Code");

        var textColumn = originalDocument.Columns.Add<StringColumn>();
        textColumn.Id = "text";
        textColumn.Name = NonLocalizedString.Create("Text");
        textColumn.Nullable = true;

        var key = originalDocument.Keys.Add();
        key.Id = "codeKey";
        key.Name = NonLocalizedString.Create("Code Key");
        key.Columns.Add(codeColumn);

        originalDocument.DefaultKey = key;

        var row1 = originalDocument.Rows.Add();
        row1["code"] = "A";
        row1["text"] = "Alpha";

        var row2 = originalDocument.Rows.Add();
        row2["code"] = "B";
        row2["text"] = null;

        await using var stream = new MemoryStream();

        await originalDocument.SaveAsync(stream, TestContext.Current.CancellationToken);

        stream.Position = 0;

        var loadedDocument = await CodeListDocument.LoadAsync(stream, TestContext.Current.CancellationToken);

        Assert.False(loadedDocument.MetaOnly);
        Assert.Equivalent(originalDocument.Comments, loadedDocument.Comments);
        Assert.Equivalent(originalDocument.Identification, loadedDocument.Identification);
        Assert.Equivalent(originalDocument.Columns, loadedDocument.Columns);
        Assert.Equal(originalDocument.Keys.Count, loadedDocument.Keys.Count);
        Assert.Equal(originalDocument.Keys[0].Id, loadedDocument.Keys[0].Id);
        Assert.Equal(originalDocument.DefaultKey.Id, loadedDocument.DefaultKey.Id);

        Assert.Equal(originalDocument.Rows.Count, loadedDocument.Rows.Count);
        Assert.Equal(originalDocument.Rows[0]["code"] as string, loadedDocument.Rows[0]["code"] as string);
        Assert.Equal(originalDocument.Rows[0]["text"] as string, loadedDocument.Rows[0]["text"] as string);
        Assert.Equal(originalDocument.Rows[1]["code"] as string, loadedDocument.Rows[1]["code"] as string);
        Assert.Equal(originalDocument.Rows[1]["text"] is null, loadedDocument.Rows[1]["text"] is null);
    }

    [Fact]
    public async Task Load_Asset_ReadsExpectedValues()
    {
        var document = await CodeListDocument.LoadAsync(
            TestAssets.GetPath("codelist.json"), 
            TestContext.Current.CancellationToken);

        new CodeListDocumentValidator().TestValidate(document).ShouldNotHaveAnyValidationErrors();

        Assert.NotNull(document);
        Assert.False(document.MetaOnly);
        Assert.Equal("This is a comment.", document.Comments[0]);
        Assert.Equal("markdown", document.Annotation.Descriptions[0].Format);
        Assert.Equal("de", document.Annotation.Descriptions[0].Language);
        Assert.Equal("Das ist eine **Anmerkung**.", document.Annotation.Descriptions[0].Content);
        Assert.Equal("TestCodeList", document.Identification.ShortName);
        Assert.Equal("A test code list", document.Identification.LongName);
        Assert.Equal("OpenPotato", document.Identification.Publisher.ShortName);
        Assert.Equal("The OpenPotato Project", document.Identification.Publisher.LongName);
        Assert.Equal("TrustMe", document.Identification.Publisher.Identifier.Source.ShortName);
        Assert.Equal("42", document.Identification.Publisher.Identifier.Value);
        Assert.Equal("de", document.Identification.AlternateLanguageLocations[0].Language);
        Assert.Equal(new Uri("https://example.com/codelist-2025-01-01.de.json"), document.Identification.AlternateLanguageLocations[0].Url);
        Assert.Equal("text/csv", document.Identification.AlternateFormatLocations[0].MimeType);
        Assert.Equal(new Uri("https://example.com/codelist-2025-01-01.csv"), document.Identification.AlternateFormatLocations[0].Url);
        Assert.True(document.Identification.Extensions.ContainsKey("x-test"));
        Assert.True(document.Identification.Publisher.Extensions.ContainsKey("x-contact-email"));
        Assert.Equal(12, document.Columns.Count);
        Assert.Equal("code", document.Columns[0].Id);
        var codeName = Assert.IsType<LocalizedString>(document.Columns[0].Name);
        Assert.Equal("Code", codeName.GetValue("en"));
        Assert.Equal("Eindeutiger Code", Assert.IsType<LocalizedString>(document.Columns[0].Description).GetValue("de"));
        Assert.Equal("en", Assert.IsType<EnumColumn>(document.Columns[5]).Language);
        Assert.Equal("e1 Wert", Assert.IsType<LocalizedString>(Assert.IsType<EnumColumn>(document.Columns[5]).Members[0].Description).GetValue("de"));
        Assert.Equal("Primärschlüssel", Assert.IsType<LocalizedString>(document.Keys[0].Name).GetValue("de"));
        Assert.Equal("Fremdschlüssel", Assert.IsType<LocalizedString>(document.ForeignKeys[0].Name).GetValue("de"));
        Assert.Equal(3, document.Rows.Count);
        Assert.Equal("c-1", document.Rows[0]["code"] as string);
        Assert.Equal("BW", document.Rows[0]["federalState"] as string);
        Assert.Equal("First", Assert.IsType<Dictionary<string, string>>(document.Rows[0]["name"])["en"]);
        Assert.Equal(42, document.Rows[0]["integer"] as long?);
        Assert.Equal(41.99M, document.Rows[0]["number"] as decimal?);
        Assert.Equal(true, document.Rows[0]["bool"] as bool?);
        Assert.Equal(["e1", "e3"], document.Rows[0]["enumSet"] as List<string>);
        Assert.Equal("c-2", document.Rows[1]["code"] as string);
        Assert.Equal("BE", document.Rows[1]["federalState"] as string);
        Assert.Equal(false, document.Rows[1]["bool"] as bool?);
        Assert.Equal([], document.Rows[1]["enumSet"] as List<string>);
    }

    [Fact]
    public async Task Save_FileRoundtrip_PreservesDocument()
    {
        var originalDocument = await CodeListDocument.LoadAsync(
            TestAssets.GetPath("codelist.json"), 
            TestContext.Current.CancellationToken);

        using var file = new TemporaryFile();
        await originalDocument.SaveAsync(file.FilePath, TestContext.Current.CancellationToken);

        var copiedDocument = await CodeListDocument.LoadAsync(
            file.FilePath,
            TestContext.Current.CancellationToken);

        Assert.Throws<FormatException>(() => copiedDocument.Rows[0]["bool"] = "string");
        Assert.Throws<FormatException>(() => copiedDocument.Rows[0]["enumSet"] = "string");

        Assert.False(originalDocument.MetaOnly);
        Assert.False(copiedDocument.MetaOnly);

        Assert.Equivalent(originalDocument.Annotation?.Descriptions, copiedDocument.Annotation?.Descriptions);
        Assert.Equivalent(originalDocument.Annotation?.AppInfo?.ToString(), copiedDocument.Annotation?.AppInfo?.ToString());
        Assert.Equivalent(originalDocument.Identification, copiedDocument.Identification);
        Assert.Equivalent(originalDocument.Columns, copiedDocument.Columns);
        Assert.Equivalent(originalDocument.Keys, copiedDocument.Keys);
        Assert.Equivalent(originalDocument.DefaultKey, copiedDocument.DefaultKey);
        Assert.Equivalent(originalDocument.ForeignKeys, copiedDocument.ForeignKeys);

        Assert.Equal(originalDocument.Rows.Count, copiedDocument.Rows.Count);
        Assert.Equal(originalDocument.Rows[0]["code"] as string, copiedDocument.Rows[0]["code"] as string);
        Assert.Equal(originalDocument.Rows[0]["federalState"] as string, copiedDocument.Rows[0]["federalState"] as string);
        Assert.Equal(originalDocument.Rows[0]["integer"] as long?, copiedDocument.Rows[0]["integer"] as long?);
        Assert.Equal(originalDocument.Rows[0]["number"] as decimal?, copiedDocument.Rows[0]["number"] as decimal?);
        Assert.Equal(originalDocument.Rows[0]["bool"] as bool?, copiedDocument.Rows[0]["bool"] as bool?);
        Assert.Equal(originalDocument.Rows[0]["enumSet"] as List<string>, copiedDocument.Rows[0]["enumSet"] as List<string>);
        Assert.Equal(originalDocument.Rows[1]["code"] as string, copiedDocument.Rows[1]["code"] as string);
        Assert.Equal(originalDocument.Rows[1]["federalState"] as string, copiedDocument.Rows[1]["federalState"] as string);
        Assert.Equal(originalDocument.Rows[1]["bool"] as bool?, copiedDocument.Rows[1]["bool"] as bool?);
        Assert.Equal(originalDocument.Rows[1]["enumSet"] as List<string>, copiedDocument.Rows[1]["enumSet"] as List<string>);
    }

    [Fact]
    public async Task SaveAsMetaOnly_FileRoundtrip_PreservesMetadata()
    {
        var originalDocument = await CodeListDocument.LoadAsync(
            TestAssets.GetPath("codelist.meta.json"), 
            TestContext.Current.CancellationToken);

        var templateDocument = await CodeListDocument.LoadAsync(
            TestAssets.GetPath("codelist.json"), 
            TestContext.Current.CancellationToken);

        using var file = new TemporaryFile();
        await templateDocument.SaveAsMetaOnlyAsync(file.FilePath, TestContext.Current.CancellationToken);

        var copiedDocument = await CodeListDocument.LoadAsync(
            file.FilePath,
            TestContext.Current.CancellationToken);

        Assert.True(originalDocument.MetaOnly);
        Assert.True(copiedDocument.MetaOnly);
        Assert.Equivalent(originalDocument.Comments, copiedDocument.Comments);
        Assert.Equivalent(originalDocument.Annotation?.Descriptions, copiedDocument.Annotation?.Descriptions);
        Assert.Equivalent(originalDocument.Annotation?.AppInfo?.ToString(), copiedDocument.Annotation?.AppInfo?.ToString());
        Assert.Equivalent(originalDocument.Identification, copiedDocument.Identification);
        Assert.Equivalent(originalDocument.Columns, copiedDocument.Columns);
        Assert.Equivalent(originalDocument.Keys, copiedDocument.Keys);
        Assert.Equivalent(originalDocument.DefaultKey, copiedDocument.DefaultKey);
        Assert.Equivalent(originalDocument.ForeignKeys, copiedDocument.ForeignKeys);

        Assert.Empty(originalDocument.Rows);
        Assert.Empty(copiedDocument.Rows);
    }
}
