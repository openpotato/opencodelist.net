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
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Tests for collection behavior of the model classes.
/// </summary>
public class CollectionTests
{
    [Fact]
    public void ColumnRefs_ColumnFromAnotherDocument_Throws()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var otherDocument = TestDocumentFactory.CreateCodeList();

        Assert.Throws<ArgumentException>(() => document.Keys[0].Columns.Add(otherDocument.Columns[0]));
    }

    [Fact]
    public void DefaultKey_KeyFromAnotherDocument_Throws()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var otherDocument = TestDocumentFactory.CreateCodeList();

        Assert.Throws<ArgumentException>(() => document.DefaultKey = otherDocument.Keys[0]);
    }

    [Fact]
    public void RemoveColumn_FilledDocument_Throws()
    {
        var document = TestDocumentFactory.CreateCodeList(addRow: true);

        Assert.Throws<InvalidOperationException>(() => document.Columns.Remove(document.Columns[0]));
    }

    [Fact]
    public void RemoveDefaultKey_ClearsDefaultKey()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var key = document.DefaultKey;

        Assert.True(document.Keys.Remove(key));
        Assert.Null(document.DefaultKey);
    }

    [Fact]
    public void RemoveReferencedColumn_EmptyDocument_RemovesKeyAndForeignKey()
    {
        var document = TestDocumentFactory.CreateCodeList();
        var codeColumn = document.Columns["code"];

        var foreignKey = document.ForeignKeys.Add();
        foreignKey.Id = "foreignKey";
        foreignKey.Columns.Add(codeColumn);
        foreignKey.KeyRef = new ExternalKeyRef
        {
            CodeListRef = new ExternalCodeListDocumentRef
            {
                CanonicalUri = new Uri("urn:test:external")
            },
            KeyId = "externalKey"
        };

        document.Columns.Remove(codeColumn);

        Assert.Empty(document.Keys);
        Assert.Empty(document.ForeignKeys);
        Assert.Empty(document.Columns);
    }
}
