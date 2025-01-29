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

using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace OpenCodeList.XUnit
{
    /// <summary>
    /// Unit tests for <see cref="CodeListSetDocument"/>.
    /// </summary>
    public class CodeListSetDocumentTest : IClassFixture<DocumentFixture>
    {
        private readonly string _assetsFolder;

        public CodeListSetDocumentTest(DocumentFixture codeListFixture)
        {
            _assetsFolder = DocumentFixture.GetAssetsFolder();
        }

        [Fact]
        public async Task Read_Test()
        {
           var document = await CodeListSetDocument.LoadAsync(Path.Combine(_assetsFolder, "codelistset.json"));

            Assert.NotNull(document);
            Assert.False(document.MetaOnly);
            Assert.Equal("This is a comment.", document.Comments[0]);
            Assert.Equal("markdown", document.Annotation.Descriptions[0].Format);
            Assert.Equal("de", document.Annotation.Descriptions[0].Language);
            Assert.Equal("Das ist eine **Anmerkung**.", document.Annotation.Descriptions[0].Content);
            Assert.Equal("TestCodeListSet", document.Identification.ShortName);
            Assert.Equal("A test code list set", document.Identification.LongName);
            Assert.Equal("OpenPotato", document.Identification.Publisher.ShortName);
            Assert.Equal("The OpenPotato Project", document.Identification.Publisher.LongName);
            Assert.Equal("TrustMe", document.Identification.Publisher.Identifier.Source.ShortName);
            Assert.Equal("42", document.Identification.Publisher.Identifier.Value);
            Assert.IsType<CodeListDocumentRef>(document.DocumentRefs[0]);
        }

        [Fact]
        public async Task Write_Test()
        {
            var originalDocument = await CodeListSetDocument.LoadAsync(Path.Combine(_assetsFolder, "codelistset.json"));

            await originalDocument.SaveAsync(Path.Combine(_assetsFolder, "codelistset.copy.json"), new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            var copiedDocument = await CodeListSetDocument.LoadAsync(Path.Combine(_assetsFolder, "codelistset.copy.json"));

            Assert.False(originalDocument.MetaOnly);
            Assert.Equivalent(originalDocument.Comments, copiedDocument.Comments);
            Assert.Equivalent(originalDocument.Annotation?.Descriptions, copiedDocument.Annotation?.Descriptions);
            Assert.Equivalent(originalDocument.Annotation?.AppInfo?.ToString(), copiedDocument.Annotation?.AppInfo?.ToString());
            Assert.Equivalent(originalDocument.Identification, copiedDocument.Identification);
            Assert.Equivalent(originalDocument.DocumentRefs, copiedDocument.DocumentRefs);
        }
    }
}
