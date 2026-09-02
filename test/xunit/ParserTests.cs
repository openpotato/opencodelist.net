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
using System.Text;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for parser and loader behavior.
/// </summary>
public class ParserTests
{
    [Theory]
    [InlineData("0.4.0")]
    [InlineData("0.4.1")]
    [InlineData("0.4.99")]
    public void CodeListDocumentLoad_Compatible04PatchVersion_Succeeds(string version)
    {
        using var stream = TestDocumentFactory.CreateCodeListAsStream(version);

        Assert.NotNull(CodeListDocument.Load(stream));
    }

    [Theory]
    [InlineData("0.3.9")]
    [InlineData("0.5.0")]
    [InlineData("1.0.0")]
    public void CodeListDocumentLoad_IncompatibleVersion_Throws(string version)
    {
        using var stream = TestDocumentFactory.CreateCodeListAsStream(version);

        Assert.Throws<CodeListParserException>(() => CodeListDocument.Load(stream));
    }

    [Fact]
    public void CodeListDocumentLoad_WrongRowJsonType_Throws()
    {
        using var stream = TestDocumentFactory.CreateCodeListAsStream(row: """{ "code": 42 }""");

        Assert.Throws<CodeListParserException>(() => CodeListDocument.Load(stream));
    }

    [Fact]
    public void CodeListDocumentLoad_UnknownRowColumn_Throws()
    {
        using var stream = TestDocumentFactory.CreateCodeListAsStream(row: """{ "unknown": "value" }""");

        Assert.Throws<CodeListParserException>(() => CodeListDocument.Load(stream));
    }

    [Fact]
    public void CodeListLoader_BothContentTypes_Throws()
    {
        const string json =
        """
        {
          "$opencodelist": "0.4.0",
          "codeList": {},
          "codeListSet": {}
        }
        """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        Assert.Throws<CodeListParserException>(() => CodeListLoader.Load(stream));
    }

    [Fact]
    public void CodeListLoader_MalformedVersion_ThrowsParserException()
    {
        using var stream = TestDocumentFactory.CreateCodeListAsStream("not-a-version");

        Assert.Throws<CodeListParserException>(() => CodeListLoader.Load(stream));
    }

    [Fact]
    public void CodeListLoader_VersionRange_MatchesTypedLoaders()
    {
        using var compatibleStream = TestDocumentFactory.CreateCodeListAsStream("0.4.7");
        Assert.IsType<CodeListDocument>(CodeListLoader.Load(compatibleStream));

        using var incompatibleStream = TestDocumentFactory.CreateCodeListAsStream("0.5.0");
        Assert.Throws<CodeListParserException>(() => CodeListLoader.Load(incompatibleStream));
    }
}
