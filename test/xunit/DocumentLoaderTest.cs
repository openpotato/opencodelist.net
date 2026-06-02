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

        public DocumentLoaderTest(DocumentFixture codeListFixture)
        {
            _assetsFolder = DocumentFixture.GetAssetsFolder();
        }

        [Fact]
        public async Task Read_CodeList()
        {
            var document = await DocumentLoader.LoadAsync(Path.Combine(_assetsFolder, "codelist.json"), TestContext.Current.CancellationToken);

            Assert.NotNull(document);
            Assert.True(document is CodeListDocument);
        }

        [Fact]
        public async Task Read_CodeListSet()
        {
            var document = await DocumentLoader.LoadAsync(Path.Combine(_assetsFolder, "codelistset.json"), TestContext.Current.CancellationToken);

            Assert.NotNull(document);
            Assert.True(document is CodeListSetDocument);
        }
    }
}
