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

using FluentValidation;

namespace OpenCodeList;

/// <summary>
/// FluentValidation validator for <see cref="CodeListSetDocument"/> instances.
/// </summary>
public sealed class CodeListSetDocumentValidator : CodeListBaseValidator<CodeListSetDocument>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeListSetDocumentValidator"/> class.
    /// </summary>
    public CodeListSetDocumentValidator()
    {
        RuleFor(document => document.DocumentRefs)
            .Must((document, references) => document.MetaOnly || references is not null && references.Count > 0)
            .WithMessage("Reference set must contain at least one document reference.")
            .OverridePropertyName(PropertyNames.ReferenceSet);

        RuleForEach(document => document.DocumentRefs)
            .Must(reference => reference is ExternalCodeListDocumentRef or ExternalCodeListSetDocumentRef)
            .WithMessage($"Document reference must be either '{TypeConsts.CodeListRef}' or '{TypeConsts.CodeListSetRef}'.")
            .SetValidator(new ExternalCodeListBaseRefValidator())
            .OverridePropertyName(PropertyNames.ReferenceSet);

        RuleFor(document => document.DocumentRefs)
            .Must((document, references) => !document.MetaOnly || references is null || references.Count == 0)
            .WithMessage("Meta document must not contain a reference set.")
            .OverridePropertyName(PropertyNames.ReferenceSet);
    }
}
