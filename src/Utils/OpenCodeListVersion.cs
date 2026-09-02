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

using Enbrea.SemVer;
using System;

namespace OpenCodeList;

/// <summary>
/// Provides methods for parsing and validating OpenCodeList version strings.
/// </summary>
public static class OpenCodeListVersion
{
    /// <summary>
    /// Parses the OpenCodeList version string.
    /// </summary>
    /// <param name="version">The OpenCodeList version string to parse.</param>
    /// <returns>The parsed <see cref="SemanticVersion"/> instance.</returns>
    public static SemanticVersion Parse(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            throw new CodeListParserException($"OpenCodeList version must not be empty.");
        }

        SemanticVersion parsedVersion;
        try
        {
            parsedVersion = SemanticVersion.Parse(version);
        }
        catch (Exception ex)
        {
            throw new CodeListParserException($"Invalid OpenCodeList version '{version}'.", ex);
        }

        if (!CodeListBase.SupportedVersionRange.Satisfies(parsedVersion))
        {
            throw new CodeListParserException($"OpenCodeList version '{version}' is not supported.");
        }

        return parsedVersion;
    }
}
