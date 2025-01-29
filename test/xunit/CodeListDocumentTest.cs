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
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace OpenCodeList.XUnit
{
    /// <summary>
    /// Unit tests for <see cref="CodeListDocument"/>.
    /// </summary>
    public class CodeListDocumentTest : IClassFixture<DocumentFixture>
    {
        private readonly string _assetsFolder;

        public CodeListDocumentTest(DocumentFixture codeListFixture)
        {
            _assetsFolder = DocumentFixture.GetAssetsFolder();
        }

        [Fact]
        public async Task Read_Test()
        {
            var document = await CodeListDocument.LoadAsync(Path.Combine(_assetsFolder, "codelist.json"));

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
            Assert.Equal(new Uri("https://example.com/codelist-2025-01-01.de.json"), document.Identification.AlternateLanguageLocations[0]);
            Assert.Equal("text/csv", document.Identification.AlternateFormatLocations[0].MimeType);
            Assert.Equal(new Uri("https://example.com/codelist-2025-01-01.csv"), document.Identification.AlternateFormatLocations[0]);
            Assert.Equal(11, document.Columns.Count);
            Assert.Equal("code", document.Columns[0].Id);
            Assert.Equal(3, document.Rows.Count);
            Assert.Equal("c-1", document.Rows[0]["code"] as string);
            Assert.Equal("BW", document.Rows[0]["federalState"] as string);
            Assert.Equal(42, document.Rows[0]["integer"] as int?);
            Assert.Equal(41.99M, document.Rows[0]["number"] as decimal?);
            Assert.Equal(true, document.Rows[0]["bool"] as bool?);
            Assert.Equal(["e1", "e3"], document.Rows[0]["enumSet"] as List<string>);
            Assert.Equal("c-2", document.Rows[1]["code"] as string);
            Assert.Equal("BE", document.Rows[1]["federalState"] as string);
            Assert.Equal(false, document.Rows[1]["bool"] as bool?);
            Assert.Equal([], document.Rows[1]["enumSet"] as List<string>);
        }

        [Fact]
        public async Task Write_Test()
        {
            var originalDocument = await CodeListDocument.LoadAsync(Path.Combine(_assetsFolder, "codelist.json"));
        
            await originalDocument.SaveAsync(Path.Combine(_assetsFolder, "codelist.copy.json"), new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            var copiedDocument = await CodeListDocument.LoadAsync(Path.Combine(_assetsFolder, "codelist.copy.json"));

            Assert.Throws<FormatException>(() => { copiedDocument.Rows[0]["bool"] = "string"; });
            Assert.Throws<FormatException>(() => { copiedDocument.Rows[0]["enumSet"] = "string"; });

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
            Assert.Equal(originalDocument.Rows[0]["integer"] as int?, copiedDocument.Rows[0]["integer"] as int?);
            Assert.Equal(originalDocument.Rows[0]["number"] as decimal?, copiedDocument.Rows[0]["number"] as decimal?);
            Assert.Equal(originalDocument.Rows[0]["bool"] as bool?, copiedDocument.Rows[0]["bool"] as bool?);
            Assert.Equal(originalDocument.Rows[0]["enumSet"] as List<string>, copiedDocument.Rows[0]["enumSet"] as List<string>);
            Assert.Equal(originalDocument.Rows[1]["code"] as string, copiedDocument.Rows[1]["code"] as string);
            Assert.Equal(originalDocument.Rows[1]["federalState"] as string, copiedDocument.Rows[1]["federalState"] as string);
            Assert.Equal(originalDocument.Rows[1]["bool"] as bool?, copiedDocument.Rows[1]["bool"] as bool?);
            Assert.Equal(originalDocument.Rows[1]["enumSet"] as List<string>, copiedDocument.Rows[1]["enumSet"] as List<string>);
        }

        [Fact]
        public async Task Meta_Test()
        {
            var originalDocument = await CodeListDocument.LoadAsync(Path.Combine(_assetsFolder, "codelist.meta.json"));

            var templateDocument = await CodeListDocument.LoadAsync(Path.Combine(_assetsFolder, "codelist.json"));

            await templateDocument.SaveAsMetaOnlyAsync(Path.Combine(_assetsFolder, "codelist.meta.copy.json"), new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            var copiedDocument = await CodeListDocument.LoadAsync(Path.Combine(_assetsFolder, "codelist.meta.copy.json"));

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
}
