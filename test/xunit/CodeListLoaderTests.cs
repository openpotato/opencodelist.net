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

using System.Threading.Tasks;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for <see cref="CodeListLoader"/>.
/// </summary>
public class CodeListLoaderTests
{
    [Fact]
    public async Task LoadAsync_CodeListFile_ReturnsCodeListDocument()
    {
        var document = await CodeListLoader.LoadAsync(
            TestAssets.GetPath("codelist.json"),
            TestContext.Current.CancellationToken);

        Assert.IsType<CodeListDocument>(document);
    }

    [Fact]
    public async Task LoadAsync_CodeListSetFile_ReturnsCodeListSetDocument()
    {
        var document = await CodeListLoader.LoadAsync(
            TestAssets.GetPath("codelistset.json"),
            TestContext.Current.CancellationToken);

        Assert.IsType<CodeListSetDocument>(document);
    }
}
