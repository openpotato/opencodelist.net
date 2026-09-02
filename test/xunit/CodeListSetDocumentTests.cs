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
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for <see cref="CodeListSetDocument"/>.
/// </summary>
public class CodeListSetDocumentTests
{

    [Fact]
    public async Task Load_MetaAsset_HasNoReferences()
    {
        var document = await CodeListSetDocument.LoadAsync(
            TestAssets.GetPath("codelistset.meta.json"),
            TestContext.Current.CancellationToken);

        new CodeListSetDocumentValidator().TestValidate(document).ShouldNotHaveAnyValidationErrors();

        Assert.True(document.MetaOnly);
        Assert.Empty(document.DocumentRefs);
    }

    [Fact]
    public async Task Load_Asset_ReadsExpectedValues()
    {
       var document = await CodeListSetDocument.LoadAsync(
           TestAssets.GetPath("codelistset.json"), 
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
    public async Task Save_FileRoundtrip_PreservesDocument()
    {
        var originalDocument = await CodeListSetDocument.LoadAsync(
            TestAssets.GetPath("codelistset.json"), 
            TestContext.Current.CancellationToken);

        using var file = new TemporaryFile();
        await originalDocument.SaveAsync(file.FilePath, TestContext.Current.CancellationToken);

        var copiedDocument = await CodeListSetDocument.LoadAsync(
            file.FilePath,
            TestContext.Current.CancellationToken);

        Assert.False(originalDocument.MetaOnly);
        Assert.Equivalent(originalDocument.Comments, copiedDocument.Comments);
        Assert.Equivalent(originalDocument.Annotation?.Descriptions, copiedDocument.Annotation?.Descriptions);
        Assert.Equivalent(originalDocument.Annotation?.AppInfo?.ToString(), copiedDocument.Annotation?.AppInfo?.ToString());
        Assert.Equivalent(originalDocument.Identification, copiedDocument.Identification);
        Assert.Equivalent(originalDocument.DocumentRefs, copiedDocument.DocumentRefs);
    }

    [Fact]
    public async Task SaveAsMetaOnly_StreamRoundtrip_ProducesValidMetaDocument()
    {
        var originalDocument = await CodeListSetDocument.LoadAsync(
            TestAssets.GetPath("codelistset.json"),
            TestContext.Current.CancellationToken);

        await using var stream = new MemoryStream();
        await originalDocument.SaveAsMetaOnlyAsync(stream, TestContext.Current.CancellationToken);
        stream.Position = 0;

        var metaDocument = await CodeListSetDocument.LoadAsync(stream, TestContext.Current.CancellationToken);
        new CodeListSetDocumentValidator().TestValidate(metaDocument).ShouldNotHaveAnyValidationErrors();

        Assert.True(metaDocument.MetaOnly);
        Assert.Empty(metaDocument.DocumentRefs);
    }
}
