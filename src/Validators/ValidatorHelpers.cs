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
using FluentValidation;
using System;

namespace OpenCodeList;

/// <summary>
/// Provides common validation methods for OpenCodeList.NET validators.
/// </summary>
internal static class ValidatorHelpers
{
    /// <summary>
    /// Determines whether the specified URI is an absolute URI or null.
    /// </summary>
    internal static bool IsAbsoluteUri(Uri uri)
    {
        return uri is null || uri.IsAbsoluteUri;
    }

    /// <summary>
    /// Determines whether the specified string is a valid BCP 47 language tag.
    /// </summary>
    internal static bool IsLanguageTag(string languageTag)
    {
        return IanaLanguageTagValidator.IsValid(languageTag);
    }

    /// <summary>
    /// Determines whether the specified string is either null, empty, or a valid BCP 47 language tag.
    /// </summary>
    internal static bool IsOptionalLanguageTag(string languageTag)
    {
        return string.IsNullOrWhiteSpace(languageTag) || IsLanguageTag(languageTag);
    }

    /// <summary>
    /// Validates a LocalizableString value, checking for null, empty, and valid BCP 47 language tags.  
    /// </summary>
    internal static void ValidateLocalizableString<T>(ValidationContext<T> context, string propertyName, LocalizableString value)
    {
        if (value is null)
        {
            context.AddFailure(propertyName, "Value must not be null.");
            return;
        }

        ValidateOptionalLocalizableString(context, propertyName, value);
    }

    /// <summary>
    /// Validates a LocalizableString value, checking for null, empty, and valid BCP 47 language tags.  
    /// </summary>
    internal static void ValidateOptionalLocalizableString<T>(ValidationContext<T> context, string propertyName, LocalizableString value)
    {
        if (value is null)
        {
            return;
        }

        switch (value)
        {
            case NonLocalizedString nonLocalized:
                if (string.IsNullOrWhiteSpace(nonLocalized.Value))
                {
                    context.AddFailure(propertyName, "Value must not be empty.");
                }
                break;

            case LocalizedString localized:
                if (localized.Values is null || localized.Values.Count == 0)
                {
                    context.AddFailure(propertyName, "At least one localized value must be specified.");
                    return;
                }

                foreach (var entry in localized.Values)
                {
                    if (!IsLanguageTag(entry.Key))
                    {
                        context.AddFailure(propertyName, $"Localized value key '{entry.Key}' must be a valid BCP 47 language tag.");
                    }

                    if (entry.Value is null)
                    {
                        context.AddFailure(propertyName, $"Localized value for '{entry.Key}' must not be null.");
                    }
                }
                break;

            default:
                context.AddFailure(propertyName, "Unsupported localizable string implementation.");
                break;
        }
    }
}

