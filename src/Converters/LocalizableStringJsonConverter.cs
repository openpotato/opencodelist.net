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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenCodeList;

/// <summary>
/// A custom JSON converter for the <see cref="LocalizableString"/> class that handles serialization and deserialization 
/// of both localized and non-localized strings.
/// </summary>
public sealed class LocalizableStringJsonConverter : JsonConverter<LocalizableString>
{
    /// <summary>
    /// Reads and converts the JSON to a <see cref="LocalizableString"/> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serialization options to use.</param>
    /// <returns>A <see cref="LocalizableString"/> object.</returns>
    public override LocalizableString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new NonLocalizedString(reader.GetString()!);
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);

            return new LocalizedString(dictionary!);
        }

        throw new JsonException("Expected a string or an object containing localized strings.");
    }

    /// <summary>
    /// Writes a <see cref="LocalizableString"/> object as JSON.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="LocalizableString"/> value to write.</param>
    /// <param name="options">The serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, LocalizableString value, JsonSerializerOptions options)
    {
        if (value is NonLocalizedString nonLocalizedString)
        {
            JsonSerializer.Serialize(writer, nonLocalizedString.Value, options);
            return;
        }

        if (value is LocalizedString localizedString)
        {
            JsonSerializer.Serialize(writer, localizedString.Values, options);
            return;
        }

        throw new JsonException("Expected a NonLocalizedString or LocalizedString.");
    }
}