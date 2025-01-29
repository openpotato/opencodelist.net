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
using System.Text.Json;

namespace OpenCodeList
{
    /// <summary>
    /// This is a time only type column. The serialized format must match the native JSON string with the JSON Schema format `time`. 
    /// See: https://json-schema.org/understanding-json-schema/reference/string.
    /// </summary>
    public class TimeOnlyColumn : Column
    {
        /// <summary>
        /// A value that specifies the maximum allowed value.
        /// </summary>
        public TimeOnly? MaxValue { get; set; }

        /// <summary>
        /// A value that specifies the minimum allowed value.
        /// </summary>
        public TimeOnly? MinValue { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="TimeOnlyColumn"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="TimeOnlyColumn"/> instance</returns>
        internal static TimeOnlyColumn Parse(JsonElement jsonElement)
        {
            var column = new TimeOnlyColumn();

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
            if (jsonElement.TryGetStringProperty(PropertyNames.MinValue, out var minValueProperty))
            {
                column.MinValue = DateTimeUtils.ParseTimeOnly(minValueProperty.GetString());
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.MaxValue, out var maxValueProperty))
            {
                column.MaxValue = DateTimeUtils.ParseTimeOnly(maxValueProperty.GetString());
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
            jsonWriter.WriteString(PropertyNames.Type, TypeConsts.TimeOnly);
            jsonWriter.WriteString(PropertyNames.Id, Id);
            jsonWriter.WriteString(PropertyNames.Name, Name);
            jsonWriter.WriteStringOrNothing(PropertyNames.Description, Description);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Nullable, Nullable);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Optional, Optional);
            jsonWriter.WriteTimeOnlyOrNothing(PropertyNames.MinValue, MinValue);
            jsonWriter.WriteTimeOnlyOrNothing(PropertyNames.MaxValue, MaxValue);
            jsonWriter.WriteEndObject();
        }
    }
}