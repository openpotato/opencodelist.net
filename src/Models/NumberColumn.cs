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
    /// This is a float type column.
    /// </summary>
    public class NumberColumn : Column
    {
        /// <summary>
        /// A number value that specifies the representing an exclusive upper limit for a value.
        /// </summary>
        public decimal? ExclusiveMaxValue { get; set; }

        /// <summary>
        /// A number value that specifies the representing an exclusive lower limit for a value.
        /// </summary>
        public decimal? ExclusiveMinValue { get; set; }

        /// <summary>
        /// A number value that specifies the minimum allowed value.
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// A number value that specifies the maximum allowed value.
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="NumberColumn"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="NumberColumn"/> instance</returns>
        internal static NumberColumn Parse(JsonElement jsonElement)
        {
            var column = new NumberColumn();

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
                column.MinValue = decimal.Parse(minValueProperty.GetString());
            }
            if (jsonElement.TryGetProperty(PropertyNames.MaxValue, out var maxValueProperty))
            {
                column.MaxValue = decimal.Parse(maxValueProperty.GetString());
            }
            if (jsonElement.TryGetProperty(PropertyNames.ExclusiveMinValue, out var exclusiveMinValueProperty))
            {
                column.MinValue = decimal.Parse(exclusiveMinValueProperty.GetString());
            }
            if (jsonElement.TryGetProperty(PropertyNames.ExclusiveMaxValue, out var exclusiveMaxValueProperty))
            {
                column.MaxValue = decimal.Parse(exclusiveMaxValueProperty.GetString());
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
            jsonWriter.WriteString(PropertyNames.Type, TypeConsts.Number);
            jsonWriter.WriteString(PropertyNames.Id, Id);
            jsonWriter.WriteString(PropertyNames.Name, Name);
            jsonWriter.WriteStringOrNothing(PropertyNames.Description, Description);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Nullable, Nullable);
            jsonWriter.WriteBooleanOrNothing(PropertyNames.Optional, Optional);
            jsonWriter.WriteNumberOrNothing(PropertyNames.MinValue, MinValue);
            jsonWriter.WriteNumberOrNothing(PropertyNames.MaxValue, MaxValue);
            jsonWriter.WriteNumberOrNothing(PropertyNames.ExclusiveMinValue, ExclusiveMinValue);
            jsonWriter.WriteNumberOrNothing(PropertyNames.ExclusiveMaxValue, ExclusiveMaxValue);
            jsonWriter.WriteEndObject();
        }
    }
}