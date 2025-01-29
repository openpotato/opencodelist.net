#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    OpenCodeList.NET  
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 * 
 */
#endregion

using System.IO;

namespace OpenCodeList.XUnit
{
    /// <summary>
    /// Class fixtures for unit tests.
    /// </summary>
    public class DocumentFixture
    {
        public static string GetOutputFolder()
        {
            // Get the full location of the assembly
            string assemblyPath = System.Reflection.Assembly.GetAssembly(typeof(DocumentFixture)).Location;

            // Get the folder that's in
            return Path.GetDirectoryName(assemblyPath);
         }

        public static string GetAssetsFolder()
        {
            // Get the folder that's in
            return Path.Combine(GetOutputFolder(), "Assets");
        }
    }
}
