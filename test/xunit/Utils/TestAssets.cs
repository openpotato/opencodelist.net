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
using System.IO;

namespace OpenCodeList.XUnit;

/// <summary>
/// Provides paths to test asset files copied to the test output directory.
/// </summary>
internal static class TestAssets
{
    public static string GetPath(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "Assets", fileName);
    }
}
