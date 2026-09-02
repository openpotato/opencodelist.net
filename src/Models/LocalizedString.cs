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
using System.Collections.Generic;

namespace OpenCodeList;

/// <summary>
/// Represents a localizable string with language-specific values.
/// </summary>
public sealed class LocalizedString : LocalizableString
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedString"/> class.
    /// </summary>
    public LocalizedString()
    {
        Values = new Dictionary<string, string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedString"/> class with language-specific values.
    /// </summary>
    /// <param name="values">The language-specific values.</param>
    public LocalizedString(IDictionary<string, string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Count == 0)
        {
            throw new ArgumentException("At least one language-specific value must be specified.", nameof(values));
        }

        Values = new Dictionary<string, string>(values);
    }

    /// <summary>
    /// Gets the language-specific values of the localized string.
    /// </summary>
    public IDictionary<string, string> Values { get; }

    /// <summary>
    /// Creates a new language-neutral string.
    /// </summary>
    /// <returns>The newly created <see cref="LocalizedString"/> instance.</returns>
    public static LocalizedString Create()
    {
        return new LocalizedString();
    }

    /// <summary>
    /// Creates a new language-neutral string.
    /// </summary>
    /// <param name="values">The language-specific values to initialize the string with.</param>
    /// <returns>The newly created <see cref="LocalizedString"/> instance.</returns>
    public static LocalizedString Create(IDictionary<string, string> values)
    {
        return new LocalizedString(values);
    }

    /// <summary>
    /// Creates a new language-neutral string.
    /// </summary>
    /// <param name="language">The language of the value to add.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The current <see cref="LocalizedString"/> instance.</returns>
    public LocalizedString AddValue(string language, string value)
    {
        Values[language] = value;
        return this;
    }

    /// <summary>
    /// Gets the value for the specified language.
    /// </summary>
    public string GetValue(string language)
    {
        if (language is null)
        {
            return null;
        }

        return Values!.TryGetValue(language, out var value) ? value : null;
    }
}