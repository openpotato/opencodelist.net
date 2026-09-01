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
/// Represents a language-neutral string that is not localized and does not have any language-specific values.
/// </summary>
public sealed class NonLocalizedString : LocalizableString
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonLocalizedString"/> class.
    /// </summary>
    /// <param name="value">The language-neutral value.</param>
    public NonLocalizedString(string value = null)
    {
        Value = value;
    }

    /// <summary>
    /// Gets or sets the language-neutral value of the string.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Creates a new language-neutral string.
    /// </summary>
    /// <param name="value">The language-neutral value.</param>
    /// <returns>The newly created <see cref="NonLocalizedString"/> instance.</returns>
    public static NonLocalizedString Create(string value)
    {
        return new NonLocalizedString(value);
    }
}