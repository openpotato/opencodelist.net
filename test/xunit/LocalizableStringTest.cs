#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*
 *    OpenCodeList.NET
 *
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License.
 */
#endregion

using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for the <see cref="LocalizableString"/> class.
/// </summary>
public class LocalizableStringTest
{
    [Fact]
    public void Converter_Rejects_Non_String_And_Non_Object_Value()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<LocalizableString>("42"));
    }

    [Fact]
    public void LocalizedString_AddValue_And_GetValue_Work()
    {
        var value = LocalizedString.Create()
            .AddValue("en", "Hello")
            .AddValue("de", "Hallo");

        Assert.Equal("Hello", value.GetValue("en"));
        Assert.Equal("Hallo", value.GetValue("de"));
        Assert.Null(value.GetValue("fr"));
        Assert.Null(value.GetValue());
    }

    [Fact]
    public void LocalizedString_Serializes_As_Language_Object()
    {
        LocalizableString value = new LocalizedString(new Dictionary<string, string>
        {
            ["en"] = "Hello",
            ["de"] = "Hallo"
        });

        var json = JsonSerializer.Serialize(value);
        var loaded = Assert.IsType<LocalizedString>(JsonSerializer.Deserialize<LocalizableString>(json));

        Assert.Equal("Hello", loaded.GetValue("en"));
        Assert.Equal("Hallo", loaded.GetValue("de"));
    }

    [Fact]
    public void NonLocalizedString_Serializes_As_Json_String()
    {
        LocalizableString value = NonLocalizedString.Create("Hello");

        var json = JsonSerializer.Serialize(value);
        var loaded = JsonSerializer.Deserialize<LocalizableString>(json);

        Assert.Equal("\"Hello\"", json);
        Assert.Equal("Hello", Assert.IsType<NonLocalizedString>(loaded).Value);
    }
}
