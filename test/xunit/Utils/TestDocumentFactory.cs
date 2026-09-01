#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*
 *    OpenCodeList.NET
 *
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License.
 */
#endregion

using System;

namespace OpenCodeList.XUnit;

/// <summary>
/// A factory class for creating test documents for unit testing purposes.
/// </summary>
internal static class TestDocumentFactory
{
    public static CodeListDocument CreateValidCodeList(bool addRow = false)
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
}
