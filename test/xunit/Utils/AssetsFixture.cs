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

using System.IO;

namespace OpenCodeList.XUnit;

/// <summary>
/// Class fixtures for unit tests.
/// </summary>
public class AssetsFixture
{
    public static string GetAssetsFolder()
    {
        // Get the folder that's in
        return Path.Combine(GetOutputFolder(), "Assets");
    }

    public static string GetOutputFolder()
    {
        // Get the full location of the assembly
        string assemblyPath = System.Reflection.Assembly.GetAssembly(typeof(AssetsFixture)).Location;

        // Get the folder that's in
        return Path.GetDirectoryName(assemblyPath);
     }
}
