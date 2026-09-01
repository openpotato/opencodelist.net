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

using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for <see cref="CodeListSetDocument"/>.
/// </summary>
public class CodeListSetTest : IClassFixture<AssetsFixture>
{
    private readonly string _assetsFolder;

    public CodeListSetTest(AssetsFixture _)
    {
        _assetsFolder = AssetsFixture.GetAssetsFolder();
    }

    [Fact]
    public async Task Load_Meta_Document_Without_ReferenceSet()
    {
        var document = await CodeListSetDocument.LoadAsync(
            Path.Combine(_assetsFolder, "codelistset.meta.json"),
            TestContext.Current.CancellationToken);

        document.Validate();

        Assert.True(document.MetaOnly);
        Assert.Empty(document.DocumentRefs);
    }

    [Fact]
    public async Task Load_Reads_All_Expected_Document_Values()
    {
       var document = await CodeListSetDocument.LoadAsync(
           Path.Combine(_assetsFolder, "codelistset.json"), 
           TestContext.Current.CancellationToken);

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
        Assert.True(document.Identification.Publisher.Extensions.ContainsKey("x-contact-email"));
        Assert.Equal(2, document.DocumentRefs.Count);
        Assert.IsType<ExternalCodeListDocumentRef>(document.DocumentRefs[0]);
        Assert.IsType<ExternalCodeListSetDocumentRef>(document.DocumentRefs[1]);
    }
    [Fact]
    public async Task Save_Writes_And_Loads_Equivalent_Document()
    {
        var originalDocument = await CodeListSetDocument.LoadAsync(
            Path.Combine(_assetsFolder, "codelistset.json"), 
            TestContext.Current.CancellationToken);

        await originalDocument.SaveAsync(
            Path.Combine(_assetsFolder, "codelistset.copy.json"),
            TestContext.Current.CancellationToken);

        var copiedDocument = await CodeListSetDocument.LoadAsync(
            Path.Combine(_assetsFolder, "codelistset.copy.json"), 
            TestContext.Current.CancellationToken);

        Assert.False(originalDocument.MetaOnly);
        Assert.Equivalent(originalDocument.Comments, copiedDocument.Comments);
        Assert.Equivalent(originalDocument.Annotation?.Descriptions, copiedDocument.Annotation?.Descriptions);
        Assert.Equivalent(originalDocument.Annotation?.AppInfo?.ToString(), copiedDocument.Annotation?.AppInfo?.ToString());
        Assert.Equivalent(originalDocument.Identification, copiedDocument.Identification);
        Assert.Equivalent(originalDocument.DocumentRefs, copiedDocument.DocumentRefs);
    }

    [Fact]
    public async Task SaveAsMetaOnly_Writes_Document_That_Can_Be_Loaded()
    {
        var originalDocument = await CodeListSetDocument.LoadAsync(
            Path.Combine(_assetsFolder, "codelistset.json"),
            TestContext.Current.CancellationToken);

        await using var stream = new MemoryStream();
        await originalDocument.SaveAsMetaOnlyAsync(stream, TestContext.Current.CancellationToken);
        stream.Position = 0;

        var metaDocument = await CodeListSetDocument.LoadAsync(stream, TestContext.Current.CancellationToken);
        metaDocument.Validate();

        Assert.True(metaDocument.MetaOnly);
        Assert.Empty(metaDocument.DocumentRefs);
    }
}
