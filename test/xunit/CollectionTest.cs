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
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Tests for collection behavior of the OpenCodeList.NET model classes.
/// </summary>
public class CollectionTest
{
    [Fact]
    public void ColumnRefs_Reject_Column_From_Another_Document()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        var otherDocument = TestDocumentFactory.CreateValidCodeList();

        Assert.Throws<ArgumentException>(() => document.Keys[0].Columns.Add(otherDocument.Columns[0]));
    }

    [Fact]
    public void DefaultKey_Rejects_Key_From_Another_Document()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        var otherDocument = TestDocumentFactory.CreateValidCodeList();

        Assert.Throws<ArgumentException>(() => document.DefaultKey = otherDocument.Keys[0]);
    }

    [Fact]
    public void Removing_Column_From_Filled_Document_Is_Rejected()
    {
        var document = TestDocumentFactory.CreateValidCodeList(addRow: true);

        Assert.Throws<InvalidOperationException>(() => document.Columns.Remove(document.Columns[0]));
    }

    [Fact]
    public void Removing_Default_Key_Clears_DefaultKey()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
        var key = document.DefaultKey;

        Assert.True(document.Keys.Remove(key));
        Assert.Null(document.DefaultKey);
    }

    [Fact]
    public void Removing_Referenced_Column_Removes_Key_And_ForeignKey_When_Document_Is_Empty()
    {
        var document = TestDocumentFactory.CreateValidCodeList();
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

        Assert.Equal(0, document.Keys.Count);
        Assert.Equal(0, document.ForeignKeys.Count);
        Assert.Equal(0, document.Columns.Count);
    }
}
