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
using System.Threading.Tasks;
using Xunit;

namespace OpenCodeList.XUnit
{
    /// <summary>
    /// Unit tests for <see cref="DocumentLoader"/>.
    /// </summary>
    public class DocumentLoaderTest : IClassFixture<DocumentFixture>
    {
        private readonly string _assetsFolder;

        public DocumentLoaderTest(DocumentFixture _)
        {
            _assetsFolder = DocumentFixture.GetAssetsFolder();
        }

        [Fact]
        public async Task LoadAsync_Returns_CodeListDocument_For_CodeList_File()
        {
            var document = await DocumentLoader.LoadAsync(Path.Combine(_assetsFolder, "codelist.json"), TestContext.Current.CancellationToken);

            Assert.NotNull(document);
            Assert.True(document is CodeListDocument);
        }

        [Fact]
        public async Task LoadAsync_Returns_CodeListSetDocument_For_CodeListSet_File()
        {
            var document = await DocumentLoader.LoadAsync(Path.Combine(_assetsFolder, "codelistset.json"), TestContext.Current.CancellationToken);

            Assert.NotNull(document);
            Assert.True(document is CodeListSetDocument);
        }
    }
}
