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

using System.Text.Json;

namespace OpenCodeList
{
    /// <summary>
    /// This is a integer type column.
    /// </summary>
    public class IntegerColumn : Column
    {
        /// <summary>
        /// An integer value that specifies the maximum allowed value.
        /// </summary>
        public int? MaxValue { get; set; }

        /// <summary>
        /// An integer value that specifies the minimum allowed value.
        /// </summary>
        public int? MinValue { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="IntegerColumn"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="IntegerColumn"/> instance</returns>
        internal static IntegerColumn Parse(JsonElement jsonElement)
        {
            var column = new IntegerColumn();

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
            if (jsonElement.TryGetProperty(PropertyNames.MinValue, out var minValueProperty))
            {
                column.MinValue = int.Parse(minValueProperty.GetString());
            }
            if (jsonElement.TryGetProperty(PropertyNames.MaxValue, out var maxValueProperty))
            {
                column.MaxValue = int.Parse(maxValueProperty.GetString());
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
            jsonWriter.WriteString(PropertyNames.Type, TypeConsts.Integer);
            jsonWriter.WriteString(PropertyNames.Id, Id);
            jsonWriter.WriteString(PropertyNames.Name, Name);
            jsonWriter.WriteStringOrNothing(PropertyNames.Description, Description);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Nullable, Nullable);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Optional, Optional);
            jsonWriter.WriteIntegerOrNothing(PropertyNames.MinValue, MinValue);
            jsonWriter.WriteIntegerOrNothing(PropertyNames.MaxValue, MaxValue);
            jsonWriter.WriteEndObject();
        }
    }
}