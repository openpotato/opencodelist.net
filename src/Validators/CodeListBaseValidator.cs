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
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenCodeList;

/// <summary>
/// Base validator for OpenCodeList documents.
/// </summary>
/// <typeparam name="TDocument">The document type.</typeparam>
public abstract class CodeListBaseValidator<TDocument> : AbstractValidator<TDocument>
    where TDocument : CodeListBase
{
    /// <summary>
    /// Initializes the validation rules shared by all OpenCodeList document types.
    /// </summary>
    protected CodeListBaseValidator()
    {
        RuleFor(document => document.Annotation)
            .SetValidator(new AnnotationValidator())
            .OverridePropertyName(PropertyNames.Annotation)
            .When(document => document.Annotation is not null);

        RuleFor(document => document.Identification)
            .NotNull()
            .WithMessage("Identification must not be null.")
            .SetValidator(new IdentificationValidator())
            .OverridePropertyName(PropertyNames.Identification);
    }
}

/// <summary>
/// Validator for <see cref="Annotation"/> instances.
/// </summary>
internal sealed class AnnotationValidator : AbstractValidator<Annotation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnnotationValidator"/> class.
    /// </summary>
    public AnnotationValidator()
    {
        RuleFor(annotation => annotation)
            .Must(annotation => annotation.AppInfo is not null || annotation.Descriptions is not null && annotation.Descriptions.Count > 0)
            .WithMessage($"Annotation must contain either '{PropertyNames.Descriptions}' or '{PropertyNames.AppInfo}'.");

        RuleForEach(annotation => annotation.Descriptions)
            .NotNull()
            .WithMessage("Description must not be null.")
            .OverridePropertyName(PropertyNames.Descriptions)
            .When(annotation => annotation.Descriptions is not null);

        RuleForEach(annotation => annotation.Descriptions)
            .SetValidator(new DescriptionValidator())
            .OverridePropertyName(PropertyNames.Descriptions)
            .When(annotation => annotation.Descriptions is not null);
    }
}

/// <summary>
/// Validator for <see cref="Description"/> instances.
/// </summary>
internal sealed class DescriptionValidator : AbstractValidator<Description>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptionValidator"/> class.
    /// </summary>
    public DescriptionValidator()
    {
        RuleFor(description => description.Language)
            .Must(ValidatorHelpers.IsOptionalLanguageTag)
            .WithMessage("Language must contain a valid BCP 47 language tag.")
            .OverridePropertyName(PropertyNames.Language);

        RuleFor(description => description.Format)
            .NotEmpty()
            .WithMessage("Format must not be empty.")
            .Must(format => format is "text" or "markdown" or "html" or "xml")
            .WithMessage(description => $"Unsupported markup format '{description.Format}'.")
            .OverridePropertyName(PropertyNames.Format);

        RuleFor(description => description.Content)
            .NotEmpty()
            .WithMessage("Content must not be empty.")
            .OverridePropertyName(PropertyNames.Content);
    }
}

/// <summary>
/// Validator for <see cref="Identification"/> instances.
/// </summary>
internal sealed class IdentificationValidator : AbstractValidator<Identification>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentificationValidator"/> class.
    /// </summary>
    public IdentificationValidator()
    {
        RuleFor(identification => identification.Language)
            .Must(ValidatorHelpers.IsOptionalLanguageTag)
            .WithMessage("Language must contain a valid BCP 47 language tag.")
            .OverridePropertyName(PropertyNames.Language);

        RuleFor(identification => identification.ShortName)
            .NotEmpty()
            .WithMessage("Short name must not be empty.")
            .OverridePropertyName(PropertyNames.ShortName);

        RuleFor(identification => identification.CanonicalUri)
            .NotNull()
            .WithMessage("Canonical URI must not be null.")
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("Canonical URI must be absolute.")
            .OverridePropertyName(PropertyNames.CanonicalUri);

        RuleFor(identification => identification.CanonicalVersionUri)
            .NotNull()
            .WithMessage("Canonical version URI must not be null.")
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("Canonical version URI must be absolute.")
            .OverridePropertyName(PropertyNames.CanonicalVersionUri);

        RuleForEach(identification => identification.Tags)
            .NotNull()
            .WithMessage("Tag must not be null.")
            .OverridePropertyName(PropertyNames.Tags)
            .When(identification => identification.Tags is not null);

        RuleForEach(identification => identification.ChangeLog)
            .NotNull()
            .WithMessage("Change-log entry must not be null.")
            .OverridePropertyName(PropertyNames.ChangeLog)
            .When(identification => identification.ChangeLog is not null);

        RuleFor(identification => identification.Publisher)
            .SetValidator(new PublisherValidator())
            .OverridePropertyName(PropertyNames.Publisher)
            .When(identification => identification.Publisher is not null);

        RuleForEach(identification => identification.LocationUrls)
            .NotNull()
            .WithMessage("Location URI must not be null.")
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("Location URI must be absolute.")
            .OverridePropertyName(PropertyNames.LocationUrls)
            .When(identification => identification.LocationUrls is not null);

        RuleForEach(identification => identification.AlternateLanguageLocations)
            .NotNull()
            .WithMessage("Alternate language location must not be null.")
            .OverridePropertyName(PropertyNames.AlternateLanguageLocations)
            .When(identification => identification.AlternateLanguageLocations is not null);

        RuleForEach(identification => identification.AlternateLanguageLocations)
            .SetValidator(new LanguageLocationValidator())
            .OverridePropertyName(PropertyNames.AlternateLanguageLocations)
            .When(identification => identification.AlternateLanguageLocations is not null);

        RuleForEach(identification => identification.AlternateFormatLocations)
            .NotNull()
            .WithMessage("Alternate format location must not be null.")
            .OverridePropertyName(PropertyNames.AlternateFormatLocations)
            .When(identification => identification.AlternateFormatLocations is not null);

        RuleForEach(identification => identification.AlternateFormatLocations)
            .SetValidator(new MimeTypedUriValidator())
            .OverridePropertyName(PropertyNames.AlternateFormatLocations)
            .When(identification => identification.AlternateFormatLocations is not null);

        RuleFor(identification => identification.ValidFrom)
            .LessThanOrEqualTo(identification => identification.ValidTo)
            .WithMessage("Valid-from must be less than or equal to valid-to.")
            .OverridePropertyName(PropertyNames.ValidFrom)
            .When(identification => identification.ValidFrom is not null && identification.ValidTo is not null);

        RuleFor(identification => identification.Extensions)
            .Custom((extensions, context) => ValidateExtensions(extensions, context));
    }

    /// <summary>
    /// Validates the extension properties of the identification.
    /// </summary>
    private static void ValidateExtensions(IDictionary<string, JsonElement> extensions, ValidationContext<Identification> context)
    {
        if (extensions is null)
        {
            return;
        }

        foreach (var extension in extensions)
        {
            if (!extension.Key.StartsWith("x-", StringComparison.Ordinal))
            {
                context.AddFailure(extension.Key, $"Extension property '{extension.Key}' must be prefixed with 'x-'.");
            }
        }
    }
}

/// <summary>
/// Validator for <see cref="IdentifierSource"/> instances.
/// </summary>
internal sealed class IdentifierSourceValidator : AbstractValidator<IdentifierSource>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentifierSourceValidator"/> class.
    /// </summary>
    public IdentifierSourceValidator()
    {
        RuleFor(source => source.ShortName)
            .NotEmpty()
            .WithMessage("Short name must not be empty.")
            .OverridePropertyName(PropertyNames.ShortName);

        RuleFor(source => source.Url)
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("URL must be absolute.")
            .OverridePropertyName(PropertyNames.Url);
    }
}

/// <summary>
/// Validator for <see cref="Identifier"/> instances.
/// </summary>
internal sealed class IdentifierValidator : AbstractValidator<Identifier>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentifierValidator"/> class.
    /// </summary>
    public IdentifierValidator()
    {
        RuleFor(identifier => identifier.Value)
            .NotEmpty()
            .WithMessage("Identifier value must not be empty.")
            .OverridePropertyName(PropertyNames.Value);

        RuleFor(identifier => identifier.Source)
            .SetValidator(new IdentifierSourceValidator())
            .OverridePropertyName(PropertyNames.Source)
            .When(identifier => identifier.Source is not null);
    }
}

/// <summary>
/// Validator for <see cref="LanguageLocation"/> instances.
/// </summary>
internal sealed class LanguageLocationValidator : AbstractValidator<LanguageLocation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageLocationValidator"/> class.
    /// </summary>
    public LanguageLocationValidator()
    {
        RuleFor(location => location.Language)
            .NotEmpty()
            .WithMessage("Language must not be empty.")
            .Must(ValidatorHelpers.IsLanguageTag)
            .WithMessage("Language must contain a valid BCP 47 language tag.")
            .OverridePropertyName(PropertyNames.Language);

        RuleFor(location => location.Url)
            .NotNull()
            .WithMessage("URL must not be null.")
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("URL must be absolute.")
            .OverridePropertyName(PropertyNames.Url);
    }
}

/// <summary>
/// Validator for <see cref="MimeTypedUri"/> instances.
/// </summary>
internal sealed class MimeTypedUriValidator : AbstractValidator<MimeTypedUri>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MimeTypedUriValidator"/> class.
    /// </summary>
    public MimeTypedUriValidator()
    {
        RuleFor(location => location.MimeType)
            .NotEmpty()
            .WithMessage("MIME type must not be empty.")
            .OverridePropertyName(PropertyNames.MimeType);

        RuleFor(location => location.Url)
            .NotNull()
            .WithMessage("URL must not be null.")
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("URL must be absolute.")
            .OverridePropertyName(PropertyNames.Url);
    }
}

/// <summary>
/// Validator for <see cref="Publisher"/> instances.
/// </summary>
internal sealed class PublisherValidator : AbstractValidator<Publisher>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherValidator"/> class.
    /// </summary>
    public PublisherValidator()
    {
        RuleFor(publisher => publisher.ShortName)
            .NotEmpty()
            .WithMessage("Short name must not be empty.")
            .OverridePropertyName(PropertyNames.ShortName);

        RuleFor(publisher => publisher.Url)
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("URL must be absolute.")
            .OverridePropertyName(PropertyNames.Url);

        RuleFor(publisher => publisher.Identifier)
            .SetValidator(new IdentifierValidator())
            .OverridePropertyName(PropertyNames.Identifier)
            .When(publisher => publisher.Identifier is not null);

        RuleFor(publisher => publisher.Extensions)
            .Custom((extensions, context) =>
            {
                if (extensions is null)
                {
                    return;
                }

                foreach (var extension in extensions)
                {
                    if (!extension.Key.StartsWith("x-", StringComparison.Ordinal))
                    {
                        context.AddFailure(extension.Key, $"Extension property '{extension.Key}' must be prefixed with 'x-'.");
                    }
                }
            });
    }
}

/// <summary>
/// Validator for <see cref="ExternalCodeListBaseRef"/> instances.
/// </summary>
internal sealed class ExternalCodeListBaseRefValidator : AbstractValidator<ExternalCodeListBaseRef>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalCodeListBaseRefValidator"/> class.
    /// </summary>
    public ExternalCodeListBaseRefValidator()
    {
        RuleFor(reference => reference.Annotation)
            .SetValidator(new AnnotationValidator())
            .OverridePropertyName(PropertyNames.Annotation)
            .When(reference => reference.Annotation is not null);

        RuleFor(reference => reference.CanonicalUri)
            .NotNull()
            .WithMessage("Canonical URI must not be null.")
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("Canonical URI must be absolute.")
            .OverridePropertyName(PropertyNames.CanonicalUri);

        RuleFor(reference => reference.CanonicalVersionUri)
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("Canonical version URI must be absolute.")
            .OverridePropertyName(PropertyNames.CanonicalVersionUri);

        RuleForEach(reference => reference.LocationUrls)
            .NotNull()
            .WithMessage("Location URI must not be null.")
            .Must(ValidatorHelpers.IsAbsoluteUri)
            .WithMessage("Location URI must be absolute.")
            .OverridePropertyName(PropertyNames.LocationUrls)
            .When(reference => reference.LocationUrls is not null);
    }
}
