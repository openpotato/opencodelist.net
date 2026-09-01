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
/// Meta information about a code list.
/// </summary>
public sealed class Identification
{
    /// <summary>
    /// Suggested retrieval locations for this document, in a format other than OpenCodeList.
    /// </summary>
    [JsonPropertyName(PropertyNames.AlternateFormatLocations)]
    [JsonPropertyOrder(16)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<MimeTypedUri> AlternateFormatLocations { get; set; }

    /// <summary>
    /// Suggested retrieval locations for this document, in OpenCodeList format, but in a different language.
    /// </summary>
    [JsonPropertyName(PropertyNames.AlternateLanguageLocations)]
    [JsonPropertyOrder(15)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<LanguageLocation> AlternateLanguageLocations { get; set; }

    /// <summary>
    /// Canonical URI which uniquely identifies all versions (collectively).
    /// </summary>
    [JsonPropertyName(PropertyNames.CanonicalUri)]
    [JsonPropertyOrder(12)]
    [JsonRequired]
    public Uri CanonicalUri { get; set; }

    /// <summary>
    /// Canonical URI which uniquely identifies this version.
    /// </summary>
    [JsonPropertyName(PropertyNames.CanonicalVersionUri)]
    [JsonPropertyOrder(13)]
    [JsonRequired]
    public Uri CanonicalVersionUri { get; set; }

    /// <summary>
    /// A curated list of notable changes for the current version of the document.
    /// </summary>
    [JsonPropertyName(PropertyNames.ChangeLog)]
    [JsonPropertyOrder(7)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> ChangeLog { get; set; }

    /// <summary>
    /// A brief description of the document.
    /// </summary>
    [JsonPropertyName(PropertyNames.Description)]
    [JsonPropertyOrder(4)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Description { get; set; }

    /// <summary>
    /// A dictionary to hold any additional properties that are not explicitly defined in the class. 
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement> Extensions { get; set; }

    /// <summary>
    /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt to specify the language of 
    /// the content. Can be overriden by the language tag of a column.
    /// </summary>
    [JsonPropertyName(PropertyNames.Language)]
    [JsonPropertyOrder(1)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Language { get; set; }

    /// <summary>
    /// Suggested retrieval location for this version, in OpenCodeList format.
    /// </summary>
    [JsonPropertyName(PropertyNames.LocationUrls)]
    [JsonPropertyOrder(14)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Uri> LocationUrls { get; set; }

    /// <summary>
    /// A human-readable name of the document.
    /// </summary>
    [JsonPropertyName(PropertyNames.LongName)]
    [JsonPropertyOrder(3)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string LongName { get; set; }

    /// <summary>
    /// The timepoint of the publication of the document.
    /// </summary>
    [JsonPropertyName(PropertyNames.PublishedAt)]
    [JsonPropertyOrder(8)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? PublishedAt { get; set; }

    /// <summary>
    /// Information about the publisher that is responsible for publication and/or maintenance of the document.
    /// </summary>
    [JsonPropertyName(PropertyNames.Publisher)]
    [JsonPropertyOrder(9)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Publisher Publisher { get; set; }

    /// <summary>
    /// An short identifier of the document.
    /// </summary>
    [JsonPropertyName(PropertyNames.ShortName)]
    [JsonPropertyOrder(2)]
    [JsonRequired]
    public string ShortName { get; set; }

    /// <summary>
    /// A list of tags or keywords that define what the document is about.
    /// </summary>
    [JsonPropertyName(PropertyNames.Tags)]
    [JsonPropertyOrder(5)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> Tags { get; set; }
    /// <summary>
    /// The timepoint from which this document is valid.
    /// </summary>
    [JsonPropertyName(PropertyNames.ValidFrom)]
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? ValidFrom { get; set; }

    /// <summary>
    /// The timepoint until which this document is valid.
    /// </summary>
    [JsonPropertyName(PropertyNames.ValidTo)]
    [JsonPropertyOrder(11)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? ValidTo { get; set; }

    /// <summary>
    /// The version of the document.
    /// </summary>
    [JsonPropertyName(PropertyNames.Version)]
    [JsonPropertyOrder(6)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Version { get; set; }
}