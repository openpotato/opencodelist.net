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
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OpenCodeList
{
    /// <summary>
    /// Extensions for <see cref="Utf8JsonWriter"/>
    /// </summary>
    public static class Utf8JsonWriterExtensions
    {
        public static void WriteAnnotation(this Utf8JsonWriter jsonWriter, string propertyName, Annotation annotation)
        {
            if (annotation != null)
            {
                jsonWriter.WritePropertyName(propertyName);
                annotation.WriteTo(jsonWriter);
            }
        }

        public static void WriteBooleanOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, bool? value)
        {
            if (value != null)
            {
                jsonWriter.WriteBoolean(propertyName, (bool)value);
            }
        }

        public static void WriteCodeListDocument(this Utf8JsonWriter jsonWriter, CodeListDocument document, bool metaOnly)
        {
            if (document != null)
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString(PropertyNames.OpenCodeList, CodeListDocument.GetVersion().ToString());
                jsonWriter.WriteStringArray(PropertyNames.Comments, document.Comments);
                jsonWriter.WritePropertyName(PropertyNames.CodeList);
                jsonWriter.WriteStartObject();
                jsonWriter.WriteAnnotation(PropertyNames.Annotation, document.Annotation);
                jsonWriter.WriteIdentification(PropertyNames.Identification, document.Identification);
                jsonWriter.WritePropertyName(PropertyNames.ColumnSet);
                jsonWriter.WriteStartObject();
                jsonWriter.WriteColumnArray(PropertyNames.Columns, document.Columns);
                jsonWriter.WriteKeyArray(PropertyNames.Keys, document.Keys);
                jsonWriter.WriteDefaultKey(PropertyNames.DefaultKey, document.DefaultKey);
                jsonWriter.WriteForeignKeyArray(PropertyNames.ForeignKeys, document.ForeignKeys);
                jsonWriter.WriteEndObject();
                if (!metaOnly)
                {
                    jsonWriter.WritePropertyName(PropertyNames.DataSet);
                    jsonWriter.WriteStartObject();
                    jsonWriter.WriteRowArray(PropertyNames.Rows, document.Rows);
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }
        }

        public static void WriteCodeListRef(this Utf8JsonWriter jsonWriter, string propertyName, CodeListDocumentRef codeListRef)
        {
            if (codeListRef != null)
            {
                jsonWriter.WritePropertyName(propertyName);
                codeListRef.WriteTo(jsonWriter);
            }
            else
            {
                jsonWriter.WriteNull(propertyName);
            }
        }

        public static void WriteCodeListSetDocument(this Utf8JsonWriter jsonWriter, CodeListSetDocument document, bool metaOnly)
        {
            if (document != null)
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString(PropertyNames.OpenCodeList, CodeListSetDocument.GetVersion().ToString());
                jsonWriter.WriteStringArray(PropertyNames.Comments, document.Comments);
                jsonWriter.WritePropertyName(PropertyNames.CodeListSet);
                jsonWriter.WriteStartObject();
                jsonWriter.WriteAnnotation(PropertyNames.Annotation, document.Annotation);
                jsonWriter.WriteIdentification(PropertyNames.Identification, document.Identification);
                if (!metaOnly)
                {
                    jsonWriter.WriteDocumentRefArray(PropertyNames.ReferenceSet, document.DocumentRefs);
                }
                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }
        }

        public static void WriteColumnArray(this Utf8JsonWriter jsonWriter, string propertyName, Columns columns)
        {
            if (columns.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var column in columns)
                {
                    column.WriteTo(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteColumnIdArray(this Utf8JsonWriter jsonWriter, string propertyName, Columns columns)
        {
            if (columns.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var column in columns)
                {
                    jsonWriter.WriteStringValue(column.Id);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteDateOnly(this Utf8JsonWriter jsonWriter, string propertyName, DateOnly? value)
        {
            if (value != null)
            {
                jsonWriter.WriteString(propertyName, value?.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo));
            }
            else
            {
                jsonWriter.WriteNull(propertyName);
            }
        }

        public static void WriteDateOnlyOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, DateOnly? value)
        {
            if (value != null)
            {
                jsonWriter.WriteString(propertyName, value?.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo));
            }
        }

        public static void WriteDateTimeOffset(this Utf8JsonWriter jsonWriter, string propertyName, DateTimeOffset? value)
        {
            if (value != null)
            {
                jsonWriter.WriteString(propertyName, value?.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFK", DateTimeFormatInfo.InvariantInfo));
            }
            else
            {
                jsonWriter.WriteNull(propertyName);
            }
        }

        public static void WriteDateTimeOffsetOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, DateTimeOffset? value)
        {
            if (value != null)
            {
                jsonWriter.WriteString(propertyName, value?.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFK", DateTimeFormatInfo.InvariantInfo));
            }
        }

        public static void WriteDefaultKey(this Utf8JsonWriter jsonWriter, string propertyName, Key defaultKey)
        {
            if (defaultKey != null)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString(PropertyNames.KeyId, defaultKey.Id);
                jsonWriter.WriteEndObject();
            }
        }

        public static void WriteDescriptionArray(this Utf8JsonWriter jsonWriter, string propertyName, IList<Description> descriptions)
        {
            if (descriptions.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var member in descriptions)
                {
                    member.WriteTo(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }
            else
            {
                jsonWriter.WriteNull(propertyName);
            }
        }

        public static void WriteDocumentRefArray(this Utf8JsonWriter jsonWriter, string propertyName, DocumentRefs members)
        {
            if (members.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var member in members)
                {
                    member.WriteTo(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteEnumMemberArray(this Utf8JsonWriter jsonWriter, string propertyName, IList<EnumMember> members)
        {
            if (members.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var member in members)
                {
                    member.WriteTo(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteForeignKeyArray(this Utf8JsonWriter jsonWriter, string propertyName, ForeignKeys foreignkeys)
        {
            if (foreignkeys.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var foreignkey in foreignkeys)
                {
                    foreignkey.WriteTo(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteIdentification(this Utf8JsonWriter jsonWriter, string propertyName, Identification identification)
        {
            if (identification != null)
            {
                jsonWriter.WritePropertyName(propertyName);
                identification.WriteTo(jsonWriter);
            }
        }

        public static void WriteIdentifier(this Utf8JsonWriter jsonWriter, string propertyName, Identifier identifier)
        {
            if (identifier != null)
            {
                jsonWriter.WritePropertyName(propertyName);
                identifier.WriteTo(jsonWriter);
            }
        }

        public static void WriteIntegerOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, int? value)
        {
            if (value != null)
            {
                jsonWriter.WriteNumber(propertyName, (int)value);
            }
        }

        public static void WriteJsonObject(this Utf8JsonWriter jsonWriter, string propertyName, JsonObject jsonObject)
        {
            if (jsonObject != null)
            {
                jsonWriter.WritePropertyName(propertyName);
                JsonSerializer.Serialize(jsonWriter, jsonObject);
            }
        }

        public static void WriteKeyArray(this Utf8JsonWriter jsonWriter, string propertyName, Keys keys)
        {
            if (keys.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var key in keys)
                {
                    key.WriteTo(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteLocalizedUriArray(this Utf8JsonWriter jsonWriter, string propertyName, IList<LocalizedUri> uris)
        {
            if (uris.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var uri in uris)
                {
                    uri.WriteTo(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteMimeTypedUriArray(this Utf8JsonWriter jsonWriter, string propertyName, IList<MimeTypedUri> uris)
        {
            if (uris.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var uri in uris)
                {
                    uri.WriteTo(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteNumberOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, decimal? value)
        {
            if (value != null)
            {
                jsonWriter.WriteNumber(propertyName, (decimal)value);
            }
        }

        public static void WriteOpenCodeListDocument(this Utf8JsonWriter jsonWriter, Document document, bool metaOnly)
        {
            if (document is CodeListDocument codeListDocument)
            {
                jsonWriter.WriteCodeListDocument(codeListDocument, metaOnly);
            }
            else if (document is CodeListSetDocument codeListSetDocument)
            {
                jsonWriter.WriteCodeListSetDocument(codeListSetDocument, metaOnly);
            }
        }

        public static void WritePublisher(this Utf8JsonWriter jsonWriter, string propertyName, Publisher publisher)
        {
            if (publisher != null)
            {
                jsonWriter.WritePropertyName(propertyName);
                publisher.WriteTo(jsonWriter);
            }
        }

        public static void WriteReference(this Utf8JsonWriter jsonWriter, string propertyName, KeyRef reference)
        {
            if (reference != null)
            {
                jsonWriter.WritePropertyName(propertyName);
                reference.WriteTo(jsonWriter);
            }
            else
            {
                jsonWriter.WriteNull(propertyName);
            }
        }

        public static void WriteRowArray(this Utf8JsonWriter jsonWriter, string propertyName, Rows rows)
        {
            if (rows.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var row in rows)
                {
                    row.WriteTo(jsonWriter);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteSource(this Utf8JsonWriter jsonWriter, string propertyName, IdentifierSource source)
        {
            if (source != null)
            {
                jsonWriter.WritePropertyName(propertyName);
                source.WriteTo(jsonWriter);
            }
        }
        public static void WriteStringArray(this Utf8JsonWriter jsonWriter, string propertyName, IList<string> list)
        {
            if (list.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var changeEntry in list)
                {
                    jsonWriter.WriteStringValue(changeEntry);
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteStringOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, string value)
        {
            if (value != null)
            {
                jsonWriter.WriteString(propertyName, value);
            }
        }

        public static void WriteTimeOnly(this Utf8JsonWriter jsonWriter, string propertyName, TimeOnly? value)
        {
            if (value != null)
            {
                jsonWriter.WriteString(propertyName, value?.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo));
            }
            else
            {
                jsonWriter.WriteNull(propertyName);
            }
        }

        public static void WriteTimeOnlyOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, TimeOnly? value)
        {
            if (value != null)
            {
                jsonWriter.WriteString(propertyName, value?.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo));
            }
        }

        public static void WriteUri(this Utf8JsonWriter jsonWriter, string propertyName, Uri value)
        {
            if (value != null)
            {
                jsonWriter.WriteString(propertyName, value.ToString());
            }
            else
            {
                jsonWriter.WriteNull(propertyName);
            }
        }

        public static void WriteUriArray(this Utf8JsonWriter jsonWriter, string propertyName, IList<Uri> uris)
        {
            if (uris.Count > 0)
            {
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartArray();
                foreach (var uri in uris)
                {
                    jsonWriter.WriteStringValue(uri.ToString());
                }
                jsonWriter.WriteEndArray();
            }
        }

        public static void WriteUriOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, Uri value)
        {
            if (value != null)
            {
                jsonWriter.WriteString(propertyName, value.ToString());
            }
        }
    }
}
