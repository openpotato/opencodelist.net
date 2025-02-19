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

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace OpenCodeList
{
    /// <summary>
    /// Meta information about a code list.
    /// </summary>
    public class Identification
    {
        /// <summary>
        /// Suggested retrieval locations for this document, in a format other than OpenCodeList.
        /// </summary>
        public IList<MimeTypedUri> AlternateFormatLocations { get; set; } = [];

        /// <summary>
        /// Suggested retrieval locations for this document, in OpenCodeList format, but in a different language.
        /// </summary>
        public IList<LocalizedUri> AlternateLanguageLocations { get; set; } = [];

        /// <summary>
        /// Canonical URI which uniquely identifies all versions (collectively).
        /// </summary>
        public Uri CanonicalUri { get; set; }

        /// <summary>
        /// Canonical URI which uniquely identifies this version.
        /// </summary>
        public Uri CanonicalVersionUri { get; set; }

        /// <summary>
        /// A curated list of notable changes for the current version of the document.
        /// </summary>
        public List<string> ChangeLog { get; } = [];

        /// <summary>
        /// A list of tags or keywords that define what the document is about.
        /// </summary>
        public List<string> Tags { get; } = [];

        /// <summary>
        /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt to specify the language of 
        /// the content. Can be overriden by the language tag of a column.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Suggested retrieval location for this version, in OpenCodeList format.
        /// </summary>
        public IList<Uri> LocationUrls { get; set; } = [];

        /// <summary>
        /// A human-readable name of the document.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// The timepoint of the publication of the document.
        /// </summary>
        public DateTimeOffset? PublishedAt { get; set; }

        /// <summary>
        /// Information about the publisher that is responsible for publication and/or maintenance of the document.
        /// </summary>
        public Publisher Publisher { get; set;  }

        /// <summary>
        /// An short identifier of the document.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The timepoint from which this document is valid.
        /// </summary>
        public DateTimeOffset? ValidFrom { get; set; }

        /// <summary>
        /// The timepoint until which this document is valid.
        /// </summary>
        public DateTimeOffset? ValidTo { get; set; }

        /// <summary>
        /// The version of the document.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="Identification"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="Identification"/> instance</returns>
        internal static Identification Parse(JsonElement jsonElement)
        {
            var identification = new Identification();

            if (jsonElement.GetRequiredStringProperty(PropertyNames.ShortName, out var shortNameProperty))
            {
                identification.ShortName = shortNameProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.LongName, out var longNameProperty))
            {
                identification.LongName = longNameProperty.GetString();
            }
            if (jsonElement.TryGetArrayProperty(PropertyNames.Tags, out var tagsProperty))
            {
                foreach (var tagsElement in tagsProperty.EnumerateArray())
                {
                    identification.Tags.Add(tagsElement.GetString());
                }
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Version, out var versionProperty))
            {
                identification.Version = versionProperty.GetString();
            }
            if (jsonElement.TryGetArrayProperty(PropertyNames.ChangeLog, out var changeLogProperty))
            {
                foreach (var changeLogElement in changeLogProperty.EnumerateArray())
                {
                    identification.ChangeLog.Add(changeLogElement.GetString());
                }
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.PublishedAt, out var publishedAtProperty))
            {
                identification.PublishedAt = DateTimeUtils.ParseDateTimeOffset(publishedAtProperty.GetString());
            }
            if (jsonElement.TryGetObjectProperty(PropertyNames.Publisher, out var publisherProperty))
            {
                identification.Publisher = Publisher.Parse(publisherProperty);
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.ValidFrom, out var validFromProperty))
            {
                identification.ValidFrom = DateTimeUtils.ParseDateTimeOffset(validFromProperty.GetString());
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.ValidTo, out var validToProperty))
            {
                identification.ValidTo = DateTimeUtils.ParseDateTimeOffset(validToProperty.GetString());
            }
            if (jsonElement.GetRequiredStringProperty(PropertyNames.CanonicalUri, out var canonicalUriProperty))
            {
                identification.CanonicalUri = new Uri(canonicalUriProperty.GetString());
            }
            if (jsonElement.GetRequiredStringProperty(PropertyNames.CanonicalVersionUri, out var canonicalVersionUriProperty))
            {
                identification.CanonicalVersionUri = new Uri(canonicalVersionUriProperty.GetString());
            }
            if (jsonElement.TryGetArrayProperty(PropertyNames.LocationUrls, out var locationUrisProperty))
            {
                foreach (var locationUriElement in locationUrisProperty.EnumerateArray())
                {
                    identification.LocationUrls.Add(new Uri(locationUriElement.GetString()));
                }
            }
            if (jsonElement.TryGetArrayProperty(PropertyNames.AlternateLanguageLocations, out var alternateLanguageLocationUrisProperty))
            {
                foreach (var alternateLanguageLocationUriElement in alternateLanguageLocationUrisProperty.EnumerateArray())
                {
                    identification.AlternateLanguageLocations.Add(LocalizedUri.Parse(alternateLanguageLocationUriElement));
                }
            }
            if (jsonElement.TryGetArrayProperty(PropertyNames.AlternateFormatLocations, out var alternateFormatLocationUrisProperty))
            {
                foreach (var alternateFormatLocationUriElement in alternateFormatLocationUrisProperty.EnumerateArray())
                {
                    identification.AlternateFormatLocations.Add(MimeTypedUri.Parse(alternateFormatLocationUriElement));
                }
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Language, out var languageProperty))
            {
                identification.Language = languageProperty.GetString();
            }

            return identification;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.ShortName, ShortName);
            jsonWriter.WriteStringOrNothing(PropertyNames.LongName, LongName);
            jsonWriter.WriteStringArray(PropertyNames.Tags, Tags);
            jsonWriter.WritePublisher(PropertyNames.Publisher, Publisher);
            jsonWriter.WriteStringOrNothing(PropertyNames.Version, Version);
            jsonWriter.WriteStringArray(PropertyNames.ChangeLog, ChangeLog);
            jsonWriter.WriteDateTimeOffsetOrNothing(PropertyNames.PublishedAt, PublishedAt);
            jsonWriter.WriteDateTimeOffsetOrNothing(PropertyNames.ValidFrom, ValidFrom);
            jsonWriter.WriteDateTimeOffsetOrNothing(PropertyNames.ValidTo, ValidTo);
            jsonWriter.WriteUri(PropertyNames.CanonicalUri, CanonicalUri);
            jsonWriter.WriteUri(PropertyNames.CanonicalVersionUri, CanonicalVersionUri);
            jsonWriter.WriteUriArray(PropertyNames.LocationUrls, LocationUrls);
            jsonWriter.WriteLocalizedUriArray(PropertyNames.AlternateLanguageLocations, AlternateLanguageLocations);
            jsonWriter.WriteMimeTypedUriArray(PropertyNames.AlternateFormatLocations, AlternateFormatLocations);
            jsonWriter.WriteStringOrNothing(PropertyNames.Language, Language);
            jsonWriter.WriteEndObject();
        }
    }
}