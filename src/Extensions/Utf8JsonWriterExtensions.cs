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

namespace OpenCodeList;

/// <summary>
/// Extensions for <see cref="Utf8JsonWriter"/>
/// </summary>
public static class Utf8JsonWriterExtensions
{
    public static void WritDocument(this Utf8JsonWriter jsonWriter, CodeListBase document, bool metaOnly)
    {
        jsonWriter.WriteStartObject();
        jsonWriter.WriteString(PropertyNames.OpenCodeList, CodeListBase.MinimumCompatibleVersion.ToString());
        jsonWriter.WriteStringArrayOrNothing(PropertyNames.Comments, document.Comments);
        jsonWriter.WritePropertyName(document is CodeListDocument ? PropertyNames.CodeList : PropertyNames.CodeListSet);
        jsonWriter.WriteStartObject();
        jsonWriter.WriteAnnotationOrNothing(PropertyNames.Annotation, document.Annotation);
        jsonWriter.WriteIdentification(PropertyNames.Identification, document.Identification);

        if (document is CodeListDocument codeListDocument)
        {
            jsonWriter.WritePropertyName(PropertyNames.ColumnSet);
            jsonWriter.WriteStartObject();
            jsonWriter.WriteColumnArray(PropertyNames.Columns, codeListDocument.Columns);
            jsonWriter.WriteKeyArrayOrNothing(PropertyNames.Keys, codeListDocument.Keys);
            jsonWriter.WriteDefaultKeyOrNothing(PropertyNames.DefaultKey, codeListDocument.DefaultKey);
            jsonWriter.WriteForeignKeyArrayOrNothing(PropertyNames.ForeignKeys, codeListDocument.ForeignKeys);
            jsonWriter.WriteEndObject();
            if (!metaOnly)
            {
                jsonWriter.WritePropertyName(PropertyNames.DataSet);
                jsonWriter.WriteStartObject();
                jsonWriter.WriteRowArray(PropertyNames.Rows, codeListDocument.Rows);
                jsonWriter.WriteEndObject();
            }
        }
        else if (document is CodeListSetDocument codeListSetDocument)
        {
            if (!metaOnly)
            {
                jsonWriter.WriteDocumentRefArrayOrNothing(PropertyNames.ReferenceSet, codeListSetDocument.DocumentRefs);
            }
        }

        jsonWriter.WriteEndObject();
        jsonWriter.WriteEndObject();
    }

    public static void WriteAnnotationOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, Annotation annotation)
    {
        if (annotation != null)
        {
            jsonWriter.WritePropertyName(propertyName);
            JsonSerializer.Serialize(jsonWriter, annotation, CodeListBase.JsonSerializerOptions);
        }
    }

    public static void WriteColumnArray(this Utf8JsonWriter jsonWriter, string propertyName, Columns columns)
    {
        jsonWriter.WritePropertyName(propertyName);
        jsonWriter.WriteStartArray();
        foreach (var column in columns)
        {
            JsonSerializer.Serialize(jsonWriter, column, CodeListBase.JsonSerializerOptions);
        }
        jsonWriter.WriteEndArray();
    }

    public static void WriteColumnRefsArrayOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, ColumnRefs columns)
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

    public static void WriteDefaultKeyOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, Key defaultKey)
    {
        if (defaultKey != null)
        {
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.KeyId, defaultKey.Id);
            jsonWriter.WriteEndObject();
        }
    }

    public static void WriteDocumentRefArrayOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, ExternalCodeListBaseRefs members)
    {
        if (members.Count > 0)
        {
            jsonWriter.WritePropertyName(propertyName);
            jsonWriter.WriteStartArray();
            foreach (var member in members)
            {
                JsonSerializer.Serialize(jsonWriter, member, CodeListBase.JsonSerializerOptions);
            }
            jsonWriter.WriteEndArray();
        }
    }

    public static void WriteForeignKeyArrayOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, ForeignKeys foreignkeys)
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
            JsonSerializer.Serialize(jsonWriter, identification, CodeListBase.JsonSerializerOptions);
        }
        else
        {
            jsonWriter.WriteNull(propertyName);
        }
    }

    public static void WriteKeyArrayOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, Keys keys)
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

    public static void WriteReference(this Utf8JsonWriter jsonWriter, string propertyName, ExternalKeyRef reference)
    {
        if (reference != null)
        {
            jsonWriter.WritePropertyName(propertyName);
            JsonSerializer.Serialize(jsonWriter, reference, CodeListBase.JsonSerializerOptions);
        }
        else
        {
            jsonWriter.WriteNull(propertyName);
        }
    }

    public static void WriteRowArray(this Utf8JsonWriter jsonWriter, string propertyName, Rows rows)
    {
        jsonWriter.WritePropertyName(propertyName);
        jsonWriter.WriteStartArray();
        foreach (var row in rows)
        {
            row.WriteTo(jsonWriter);
        }
        jsonWriter.WriteEndArray();
    }

    public static void WriteStringArrayOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, IList<string> list)
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

    public static void WriteStringOrNothing(this Utf8JsonWriter jsonWriter, string propertyName, LocalizableString value)
    {
        if (value != null)
        {
            jsonWriter.WritePropertyName(propertyName);
            JsonSerializer.Serialize(jsonWriter, value, CodeListBase.JsonSerializerOptions);
        }
    }
}
