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

using Enbrea.Bcp47.Iana;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenCodeList;

/// <summary>
/// Base class for validating <see cref="CodeListBase"/> instances.
/// </summary>
public abstract class CodeListBaseValidator
{
    private readonly CodeListBase _document;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeListBaseValidator"/> class.
    /// </summary>
    /// <param name="document">The <see cref="CodeListBase"/> instance to validate.</param>
    public CodeListBaseValidator(CodeListBase document)
    {
        _document = document;
    }

    /// <summary>
    /// Validates the <see cref="CodeListBase"/> instance.
    /// </summary>
    public virtual void Validate()
    {
        ValidateCommentList(_document.Comments, PropertyNames.Comments);
        ValidateAnnotation(_document.Annotation, PropertyNames.Annotation);
        ValidateIdentification(_document.Identification, PropertyNames.Identification);
    }

    /// <summary>
    /// Validates the <see cref="Annotation"/> instance.
    /// </summary>
    protected static void ValidateAnnotation(Annotation annotation, string propertyPath)
    {
        if (annotation is null)
        {
            return;
        }

        var hasDescriptions = annotation.Descriptions is not null && annotation.Descriptions.Count > 0;
        var hasAppInfo = annotation.AppInfo is not null;

        if (!hasDescriptions && !hasAppInfo)
        {
            throw new CodeListValidatorException($"'{propertyPath}' must contain either '{PropertyNames.Descriptions}' or '{PropertyNames.AppInfo}'.");
        }

        if (annotation.Descriptions is null)
        {
            return;
        }

        for (var i = 0; i < annotation.Descriptions.Count; i++)
        {
            var description = annotation.Descriptions[i];
            var itemPath = $"{propertyPath}.{PropertyNames.Descriptions}[{i}]";

            if (description is null)
            {
                throw new CodeListValidatorException($"'{itemPath}' must not be null.");
            }

            ValidateLanguageTag(description.Language, $"{itemPath}.{PropertyNames.Language}", false);
            ValidateRequiredString(description.Format, $"{itemPath}.{PropertyNames.Format}");
            ValidateRequiredString(description.Content, $"{itemPath}.{PropertyNames.Content}");

            if (description.Format is not "text" and not "markdown" and not "html" and not "xml")
            {
                throw new CodeListValidatorException($"'{itemPath}.{PropertyNames.Format}' contains unsupported markup format '{description.Format}'.");
            }
        }
    }

    /// <summary>
    /// Validates a list of comments.
    /// </summary>
    protected static void ValidateCommentList(IList<string> comments, string propertyPath)
    {
        for (var i = 0; i < comments.Count; i++)
        {
            if (comments[i] is null)
            {
                throw new CodeListValidatorException($"'{propertyPath}[{i}]' must not be null.");
            }
        }
    }

    /// <summary>
    /// Validates the <see cref="ExternalCodeListBaseRef"/> instance.
    /// </summary>
    protected static void ValidateExternalCodeListRef(ExternalCodeListBaseRef codeListRef, string propertyPath)
    {
        if (codeListRef is null)
        {
            throw new CodeListValidatorException($"'{propertyPath}' must not be null.");
        }

        ValidateAnnotation(codeListRef.Annotation, $"{propertyPath}.{PropertyNames.Annotation}");
        ValidateRequiredUri(codeListRef.CanonicalUri, $"{propertyPath}.{PropertyNames.CanonicalUri}");
        ValidateOptionalUri(codeListRef.CanonicalVersionUri, $"{propertyPath}.{PropertyNames.CanonicalVersionUri}");
        ValidateUriList(codeListRef.LocationUrls, $"{propertyPath}.{PropertyNames.LocationUrls}", false);
    }

    /// <summary>
    /// Validates the <see cref="Identification"/> instance.
    /// </summary>
    protected static void ValidateIdentification(Identification identification, string propertyPath)
    {
        if (identification is null)
        {
            throw new CodeListValidatorException($"'{propertyPath}' must not be null.");
        }

        ValidateLanguageTag(identification.Language, $"{propertyPath}.{PropertyNames.Language}", false);
        ValidateRequiredString(identification.ShortName, $"{propertyPath}.{PropertyNames.ShortName}");
        ValidateRequiredUri(identification.CanonicalUri, $"{propertyPath}.{PropertyNames.CanonicalUri}");
        ValidateRequiredUri(identification.CanonicalVersionUri, $"{propertyPath}.{PropertyNames.CanonicalVersionUri}");
        ValidateStringList(identification.Tags, $"{propertyPath}.{PropertyNames.Tags}", false);
        ValidateStringList(identification.ChangeLog, $"{propertyPath}.{PropertyNames.ChangeLog}", false);
        ValidatePublisher(identification.Publisher, $"{propertyPath}.{PropertyNames.Publisher}");
        ValidateUriList(identification.LocationUrls, $"{propertyPath}.{PropertyNames.LocationUrls}", false);

        if (identification.AlternateLanguageLocations is not null)
        {
            for (var i = 0; i < identification.AlternateLanguageLocations.Count; i++)
            {
                var languageLocation = identification.AlternateLanguageLocations[i];
                var itemPath = $"{propertyPath}.{PropertyNames.AlternateLanguageLocations}[{i}]";

                if (languageLocation is null)
                {
                    throw new CodeListValidatorException($"'{itemPath}' must not be null.");
                }

                ValidateLanguageTag(languageLocation.Language, $"{itemPath}.{PropertyNames.Language}", true);
                ValidateRequiredUri(languageLocation.Url, $"{itemPath}.{PropertyNames.Url}");
            }
        }

        if (identification.AlternateFormatLocations is not null)
        {
            for (var i = 0; i < identification.AlternateFormatLocations.Count; i++)
            {
                var typedUri = identification.AlternateFormatLocations[i];
                var itemPath = $"{propertyPath}.{PropertyNames.AlternateFormatLocations}[{i}]";

                if (typedUri is null)
                {
                    throw new CodeListValidatorException($"'{itemPath}' must not be null.");
                }

                ValidateRequiredString(typedUri.MimeType, $"{itemPath}.{PropertyNames.MimeType}");
                ValidateRequiredUri(typedUri.Url, $"{itemPath}.{PropertyNames.Url}");
            }
        }

        if (identification.ValidFrom is not null && identification.ValidTo is not null && identification.ValidFrom > identification.ValidTo)
        {
            throw new CodeListValidatorException($"'{propertyPath}.{PropertyNames.ValidFrom}' must be less than or equal to '{propertyPath}.{PropertyNames.ValidTo}'.");
        }

        if (identification.Extensions is not null)
        {
            foreach (var extension in identification.Extensions)
            {
                if (!extension.Key.StartsWith("x-", StringComparison.Ordinal))
                {
                    throw new CodeListValidatorException($"Extension property '{extension.Key}' must be prefixed with 'x-'.");
                }
            }
        }
    }

    /// <summary>
    /// Validates a language tag according to BCP 47.
    /// </summary>
    protected static void ValidateLanguageTag(string languageTag, string propertyPath, bool required)
    {
        if (string.IsNullOrWhiteSpace(languageTag))
        {
            if (required)
            {
                throw new CodeListValidatorException($"'{propertyPath}' must not be empty.");
            }

            return;
        }

        if (!IanaLanguageTagValidator.IsValid(languageTag))
        {
            throw new CodeListValidatorException($"'{propertyPath}' must contain a valid language tag.");
        }

        var parts = languageTag.Split('-');
        if (parts.Any(part => string.IsNullOrWhiteSpace(part)))
        {
            throw new CodeListValidatorException($"'{propertyPath}' must contain a valid language tag.");
        }
    }

    /// <summary>
    /// Validates a <see cref="LocalizableString"/> instance.   
    /// </summary>
    protected static void ValidateLocalizableString(LocalizableString value, string propertyPath, bool required)
    {
        if (value is null)
        {
            if (required)
            {
                throw new CodeListValidatorException($"'{propertyPath}' must not be null.");
            }

            return;
        }

        if (value is NonLocalizedString nonLocalizedString)
        {
            ValidateRequiredString(nonLocalizedString.Value, propertyPath);
            return;
        }

        if (value is LocalizedString localizedString)
        {
            if (localizedString.Values is null || localizedString.Values.Count == 0)
            {
                throw new CodeListValidatorException($"'{propertyPath}' must contain at least one localized value.");
            }

            foreach (var localizedValue in localizedString.Values)
            {
                ValidateLanguageTag(localizedValue.Key, $"{propertyPath}[{localizedValue.Key}]", true);
                ValidateRequiredString(localizedValue.Value, $"{propertyPath}[{localizedValue.Key}]", false);
            }

            return;
        }

        throw new CodeListValidatorException($"'{propertyPath}' contains an unsupported localizable string implementation.");
    }

    /// <summary>
    /// Validates an optional URI. If the URI is not null, it must be an absolute URI.
    /// </summary>
    protected static void ValidateOptionalUri(Uri uri, string propertyPath)
    {
        if (uri is null)
        {
            return;
        }

        if (!uri.IsAbsoluteUri)
        {
            throw new CodeListValidatorException($"'{propertyPath}' must contain an absolute URI.");
        }
    }

    /// <summary>
    /// Validates a <see cref="Publisher"/> instance.
    /// </summary>
    protected static void ValidatePublisher(Publisher publisher, string propertyPath)
    {
        if (publisher is null)
        {
            return;
        }

        ValidateRequiredString(publisher.ShortName, $"{propertyPath}.{PropertyNames.ShortName}");
        ValidateOptionalUri(publisher.Url, $"{propertyPath}.{PropertyNames.Url}");

        if (publisher.Extensions is not null)
        {
            foreach (var extension in publisher.Extensions)
            {
                if (!extension.Key.StartsWith("x-", StringComparison.Ordinal))
                {
                    throw new CodeListValidatorException($"Extension property '{extension.Key}' must be prefixed with 'x-'.");
                }
            }
        }

        if (publisher.Identifier is null)
        {
            return;
        }

        ValidateRequiredString(publisher.Identifier.Value, $"{propertyPath}.{PropertyNames.Identifier}.{PropertyNames.Value}");

        if (publisher.Identifier.Source is null)
        {
            return;
        }

        ValidateRequiredString(publisher.Identifier.Source.ShortName, $"{propertyPath}.{PropertyNames.Identifier}.{PropertyNames.Source}.{PropertyNames.ShortName}");
        ValidateOptionalUri(publisher.Identifier.Source.Url, $"{propertyPath}.{PropertyNames.Identifier}.{PropertyNames.Source}.{PropertyNames.Url}");
    }

    /// <summary>
    /// Validates a required string. If the string is null or empty (after trimming), an exception is thrown.
    /// </summary>
    protected static void ValidateRequiredString(string value, string propertyPath, bool trim = true)
    {
        if (trim)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new CodeListValidatorException($"'{propertyPath}' must not be empty.");
            }
        }
        else if (value is null)
        {
            throw new CodeListValidatorException($"'{propertyPath}' must not be null.");
        }
    }

    /// <summary>
    /// Validates a required URI. If the URI is null or not absolute, an exception is thrown.
    /// </summary>
    protected static void ValidateRequiredUri(Uri uri, string propertyPath)
    {
        if (uri is null)
        {
            throw new CodeListValidatorException($"'{propertyPath}' must not be null.");
        }

        ValidateOptionalUri(uri, propertyPath);
    }

    /// <summary>
    /// Validates a list of strings. If the list is null, empty (when required), or contains null/empty strings, an exception is thrown.
    /// </summary>
    protected static void ValidateStringList(IList<string> values, string propertyPath, bool requireNonEmpty)
    {
        if (values is null)
        {
            if (requireNonEmpty)
            {
                throw new CodeListValidatorException($"'{propertyPath}' must not be null.");
            }

            return;
        }

        if (requireNonEmpty && values.Count == 0)
        {
            throw new CodeListValidatorException($"'{propertyPath}' must not be empty.");
        }

        for (var i = 0; i < values.Count; i++)
        {
            ValidateRequiredString(values[i], $"{propertyPath}[{i}]", false);
        }
    }

    /// <summary>
    /// Validates a list of URIs. If the list is null, empty (when required), or contains null/invalid URIs, an exception is thrown.
    /// </summary>
    protected static void ValidateUriList(IList<Uri> values, string propertyPath, bool requireNonEmpty)
    {
        if (values is null)
        {
            if (requireNonEmpty)
            {
                throw new CodeListValidatorException($"'{propertyPath}' must not be null.");
            }

            return;
        }

        if (requireNonEmpty && values.Count == 0)
        {
            throw new CodeListValidatorException($"'{propertyPath}' must not be empty.");
        }

        for (var i = 0; i < values.Count; i++)
        {
            ValidateRequiredUri(values[i], $"{propertyPath}[{i}]");
        }
    }
}