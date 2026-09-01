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

using System.Text.Json;

namespace OpenCodeList;

/// <summary>
/// Extensions for <see cref="JsonElement"/>
/// </summary>
public static class JsonElementExtensions
{
    public static bool GetRequiredArrayProperty(ref this JsonElement jsonElement, string propertyName, out JsonElement value)
    {
        if (!jsonElement.TryGetArrayProperty(propertyName, out value))
        {
            throw new CodeListParserException($"Required property \"{propertyName}\" not found.");
        }
        return true;
    }

    public static bool GetRequiredObjectProperty(ref this JsonElement jsonElement, string propertyName, out JsonElement value)
    {
        if (!jsonElement.TryGetObjectProperty(propertyName, out value))
        {
            throw new CodeListParserException($"Required property \"{propertyName}\" not found.");
        }
        return true;
    }

    public static bool GetRequiredStringProperty(ref this JsonElement jsonElement, string propertyName, out JsonElement value)
    {
        if (!jsonElement.TryGetStringProperty(propertyName, out value))
        {
            throw new CodeListParserException($"Required property \"{propertyName}\" not found.");
        }
        return true;
    }

    public static bool TryGetArrayProperty(ref this JsonElement jsonElement, string propertyName, out JsonElement value)
    {
        if (jsonElement.TryGetProperty(propertyName, out value))
        {
            if (value.ValueKind != JsonValueKind.Array)
            {
                throw new CodeListParserException($"Property \"{propertyName}\" must be a JSON array.");
            }
            return true;
        }
        return false;
    }

    public static bool TryGetObjectProperty(ref this JsonElement jsonElement, string propertyName, out JsonElement value)
    {
        if (jsonElement.TryGetProperty(propertyName, out value))
        {
            if (value.ValueKind != JsonValueKind.Object)
            {
                throw new CodeListParserException($"Property \"{propertyName}\" must be a JSON object.");
            }
            return true;
        }
        return false;
    }

    public static bool TryGetStringProperty(ref this JsonElement jsonElement, string propertyName, out JsonElement value)
    {
        if (jsonElement.TryGetProperty(propertyName, out value))
        {
            if (value.ValueKind != JsonValueKind.String)
            {
                throw new CodeListParserException($"Property \"{propertyName}\" must be a JSON String.");
            }
            return true;
        }
        return false;
    }
}
