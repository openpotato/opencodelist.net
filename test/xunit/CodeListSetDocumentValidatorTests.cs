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
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for <see cref="CodeListSetDocumentValidator"/>.
/// </summary>
public class CodeListSetDocumentValidatorTests
{
    private readonly CodeListSetDocumentValidator _validator = new();

    [Fact]
    public void EmptyMetaCodeListSet_IsValid()
    {
        var document = TestDocumentFactory.CreateCodeListSet(metaOnly: true);

        _validator.TestValidate(document)
            .ShouldNotHaveAnyValidationErrors();

        Assert.True(document.MetaOnly);
    }

    [Fact]
    public void EmptyNonMetaCodeListSet_IsInvalid()
    {
        var document = TestDocumentFactory.CreateCodeListSet();

        Assert.False(document.MetaOnly);
        
        _validator.TestValidate(document)
            .ShouldHaveValidationErrorFor("referenceSet");
    }
}
