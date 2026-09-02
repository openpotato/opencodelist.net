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

using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace OpenCodeList.XUnit;

/// <summary>
/// Unit tests for the <see cref="LocalizableString"/> class.
/// </summary>
public class LocalizableStringTests
{
    [Fact]
    public void Converter_NonStringAndNonObjectValue_Throws()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<LocalizableString>("42"));
    }

    [Fact]
    public void LocalizedString_AddAndGetValue_Works()
    {
        var value = LocalizedString.Create()
            .AddValue("en", "Hello")
            .AddValue("de", "Hallo");

        Assert.Equal("Hello", value.GetValue("en"));
        Assert.Equal("Hallo", value.GetValue("de"));
        Assert.Null(value.GetValue("fr"));
    }

    [Fact]
    public void LocalizedString_Serialization_UsesLanguageObject()
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
    public void NonLocalizedString_Serialization_UsesJsonString()
    {
        LocalizableString value = NonLocalizedString.Create("Hello");

        var json = JsonSerializer.Serialize(value);
        var loaded = JsonSerializer.Deserialize<LocalizableString>(json);

        Assert.Equal("\"Hello\"", json);
        Assert.Equal("Hello", Assert.IsType<NonLocalizedString>(loaded).Value);
    }
}
