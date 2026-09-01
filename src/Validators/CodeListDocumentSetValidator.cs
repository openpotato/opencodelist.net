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

namespace OpenCodeList;

/// <summary>
/// An abstract OpenCodeList base class for <see cref="CodeListDocument"/> and
/// <see cref="CodeListSetDocument"/> 
/// </summary>
public sealed class CodeListDocumentSetValidator : CodeListBaseValidator
{
    private readonly CodeListSetDocument _document;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeListDocumentSetValidator"/> class.
    /// </summary>
    public CodeListDocumentSetValidator(CodeListSetDocument document)
        : base(document)
    {
        _document = document;
    }

    /// <summary>
    /// Validates the <see cref="CodeListSetDocument"/> instance.
    /// </summary>
    public override void Validate()
    {
        base.Validate();

        if (_document.DocumentRefs is null)
        {
            throw new CodeListValidatorException($"'{PropertyNames.ReferenceSet}' must not be null.");
        }

        if (!_document.MetaOnly && _document.DocumentRefs.Count == 0)
        {
            throw new CodeListValidatorException($"'{PropertyNames.ReferenceSet}' must contain at least one document reference.");
        }

        for (var i = 0; i < _document.DocumentRefs.Count; i++)
        {
            var documentRef = _document.DocumentRefs[i];
            var path = $"{PropertyNames.ReferenceSet}[{i}]";

            if (documentRef is null)
            {
                throw new CodeListValidatorException($"'{path}' must not be null.");
            }

            if (documentRef is not ExternalCodeListDocumentRef && documentRef is not ExternalCodeListSetDocumentRef)
            {
                throw new CodeListValidatorException($"'{path}' must be either '{TypeConsts.CodeListRef}' or '{TypeConsts.CodeListSetRef}'.");
            }

            ValidateExternalCodeListRef(documentRef, path);
        }

        if (_document.MetaOnly && _document.DocumentRefs.Count > 0)
        {
            throw new CodeListValidatorException($"Meta document must not contain '{PropertyNames.ReferenceSet}'.");
        }
    }
}