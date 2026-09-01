#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*
 *    OpenCodeList.NET
 *
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License.
 */
#endregion

using System.IO;
using System.Text;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for the OpenCodeList.NET parser and loader.
/// </summary>
public class ParserTest
{
    [Theory]
    [InlineData("0.4.0")]
    [InlineData("0.4.1")]
    [InlineData("0.4.99")]
    public void CodeListDocument_Load_Accepts_All_Compatible_04_Patch_Versions(string version)
    {
        using var stream = ToStream(CreateCodeListJson(version));

        var document = CodeListDocument.Load(stream);

        Assert.NotNull(document);
    }

    [Theory]
    [InlineData("0.3.9")]
    [InlineData("0.5.0")]
    [InlineData("1.0.0")]
    public void CodeListDocument_Load_Rejects_Incompatible_Versions(string version)
    {
        using var stream = ToStream(CreateCodeListJson(version));

        Assert.Throws<CodeListParserException>(() => CodeListDocument.Load(stream));
    }

    [Fact]
    public void CodeListDocument_Load_Rejects_Row_Value_With_Wrong_Json_Type()
    {
        var json = CreateCodeListJson("0.4.0", """{ "code": 42 }""");
        using var stream = ToStream(json);

        Assert.Throws<CodeListParserException>(() => CodeListDocument.Load(stream));
    }

    [Fact]
    public void CodeListDocument_Load_Rejects_Unknown_Row_Column()
    {
        var json = CreateCodeListJson("0.4.0", """{ "unknown": "value" }""");
        using var stream = ToStream(json);

        Assert.Throws<CodeListParserException>(() => CodeListDocument.Load(stream));
    }

    [Fact]
    public void CodeListDocument_Validate_Rejects_Missing_Required_Row_Value()
    {
        var json = CreateCodeListJson("0.4.0", "{}");
        using var stream = ToStream(json);

        var document = CodeListDocument.Load(stream);

        Assert.Throws<CodeListValidatorException>(() => document.Validate());
    }

    [Fact]
    public void CodeListLoader_Rejects_Document_With_Both_Content_Types()
    {
        const string json = """
        {
          "$opencodelist": "0.4.0",
          "codeList": {},
          "codeListSet": {}
        }
        """;

        using var stream = ToStream(json);

        Assert.Throws<CodeListParserException>(() => CodeListLoader.Load(stream));
    }

    [Fact]
    public void CodeListLoader_Rejects_Malformed_Version_With_Parser_Exception()
    {
        using var stream = ToStream(CreateCodeListJson("not-a-version"));

        Assert.Throws<CodeListParserException>(() => CodeListLoader.Load(stream));
    }

    [Fact]
    public void CodeListLoader_Uses_Same_Version_Range_As_Typed_Loaders()
    {
        using var compatibleStream = ToStream(CreateCodeListJson("0.4.7"));
        Assert.IsType<CodeListDocument>(CodeListLoader.Load(compatibleStream));

        using var incompatibleStream = ToStream(CreateCodeListJson("0.5.0"));
        Assert.Throws<CodeListParserException>(() => CodeListLoader.Load(incompatibleStream));
    }

    private static string CreateCodeListJson(string version, string row = null)
    {
        var rows = row is null ? "[]" : $"[{row}]";

        return $$"""
        {
          "$opencodelist": "{{version}}",
          "codeList": {
            "identification": {
              "shortName": "Test",
              "canonicalUri": "urn:test:codelist",
              "canonicalVersionUri": "urn:test:codelist:1"
            },
            "columnSet": {
              "columns": [
                {
                  "id": "code",
                  "name": "Code",
                  "type": "string",
                  "nullable": false,
                  "optional": false
                }
              ],
              "keys": [
                {
                  "id": "codeKey",
                  "columnIds": ["code"]
                }
              ]
            },
            "dataSet": {
              "rows": {{rows}}
            }
          }
        }
        """;
    }

    private static MemoryStream ToStream(string value)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(value));
    }
}
