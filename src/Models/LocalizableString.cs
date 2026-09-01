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

using System.Text.Json.Serialization;

namespace OpenCodeList;

/// <summary>
/// Represents a string that can be localized to different languages. 
/// </summary>
[JsonConverter(typeof(LocalizableStringJsonConverter))]
public abstract class LocalizableString
{
}