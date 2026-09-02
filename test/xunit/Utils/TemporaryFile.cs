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
/// Represents a temporary file that is deleted when disposed.
/// </summary>
internal sealed class TemporaryFile : IDisposable
{
    public TemporaryFile()
    {
        FilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    }

    public string FilePath { get; }

    public void Dispose()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }
}
