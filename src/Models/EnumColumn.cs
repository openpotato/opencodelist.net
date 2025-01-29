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

using System.Collections.Generic;
using System.Text.Json;

namespace OpenCodeList
{
    /// <summary>
    /// An enumeration type column
    /// </summary>
    public class EnumColumn : Column
    {
        /// <summary>
        /// A language tag according to https://www.rfc-editor.org/rfc/bcp/bcp47.txt to specify the language of the content.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The list of allowed values for this enumeration.
        /// </summary>
        public IList<EnumMember> Members { get; } = [];

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="EnumColumn"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="EnumColumn"/> instance</returns>
        internal static EnumColumn Parse(JsonElement jsonElement)
        {
            var column = new EnumColumn();

            if (jsonElement.GetRequiredStringProperty(PropertyNames.Id, out var idProperty))
            {
                column.Id = idProperty.GetString();
            }
            if (jsonElement.GetRequiredStringProperty(PropertyNames.Name, out var nameProperty))
            {
                column.Name = nameProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Description, out var descriptionProperty))
            {
                column.Description = descriptionProperty.GetString();
            }
            if (jsonElement.TryGetBooleanProperty(PropertyNames.Nullable, out var nullableProperty))
            {
                column.Nullable = nullableProperty.GetBoolean();
            }
            if (jsonElement.TryGetBooleanProperty(PropertyNames.Optional, out var optionalProperty))
            {
                column.Optional = optionalProperty.GetBoolean();
            }
            if (jsonElement.TryGetProperty(PropertyNames.Members, out var membersProperty))
            {
                if (membersProperty.ValueKind == JsonValueKind.Array)
                {
                    foreach (var memberElement in membersProperty.EnumerateArray())
                    {
                        if (memberElement.ValueKind == JsonValueKind.Object)
                        {
                            column.Members.Add(EnumMember.Parse(memberElement));
                        }
                    }
                }
            }
            if (jsonElement.TryGetProperty(PropertyNames.Language, out var languageProperty))
            {
                column.Language = languageProperty.GetString();
            }

            return column;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal override void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.Type, TypeConsts.Enum);
            jsonWriter.WriteString(PropertyNames.Id, Id);
            jsonWriter.WriteString(PropertyNames.Name, Name);
            jsonWriter.WriteStringOrNothing(PropertyNames.Description, Description);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Nullable, Nullable);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Optional, Optional);
            jsonWriter.WriteEnumMemberArray(PropertyNames.Members, Members);
            jsonWriter.WriteStringOrNothing(PropertyNames.Language, Language);
            jsonWriter.WriteEndObject();
        }
    }
}