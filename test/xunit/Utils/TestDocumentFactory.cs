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

using System;
using System.IO;
using System.Text;

namespace OpenCodeList.XUnit;

/// <summary>
/// Creates minimal OpenCodeList documents used by unit tests.
/// </summary>
internal static class TestDocumentFactory
{
    public static CodeListDocument CreateCodeList(bool addRow = false)
    {
        var document = new CodeListDocument();

        document.Identification.ShortName = "Test";
        document.Identification.CanonicalUri = new Uri("urn:test:codelist");
        document.Identification.CanonicalVersionUri = new Uri("urn:test:codelist:1");

        var codeColumn = document.Columns.Add<StringColumn>();
        codeColumn.Id = "code";
        codeColumn.Name = NonLocalizedString.Create("Code");
        codeColumn.Nullable = false;
        codeColumn.Optional = false;

        var key = document.Keys.Add();
        key.Id = "codeKey";
        key.Columns.Add(codeColumn);
        document.DefaultKey = key;

        if (addRow)
        {
            var row = document.Rows.Add();
            row[codeColumn.Id] = "A";
        }

        return document;
    }

    public static string CreateCodeListAsJson(string version = "0.4.0", string row = null)
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

    public static MemoryStream CreateCodeListAsStream(string version = "0.4.0", string row = null)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(CreateCodeListAsJson(version, row)));
    }

    public static CodeListSetDocument CreateCodeListSet(bool metaOnly = false)
    {
        var document = new CodeListSetDocument();

        document.Identification.ShortName = "Set";
        document.Identification.CanonicalUri = new Uri("urn:test:set");
        document.Identification.CanonicalVersionUri = new Uri("urn:test:set:1");

        if (metaOnly)
        {
            document.ClearContent(true);
        }

        return document;
    }
}
