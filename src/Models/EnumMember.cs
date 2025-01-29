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
    /// An enumeration member.
    /// </summary>
    public class EnumMember
    {
        /// <summary>
        /// A short description of the value.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Parses a <see cref="JsonElement"/> object into a new <see cref="EnumMember"/> instance.
        /// </summary>
        /// <param name="jsonElement">The json object</param>
        /// <returns>A new <see cref="EnumMember"/> instance</returns>
        internal static EnumMember Parse(JsonElement jsonElement)
        {
            var enumMember = new EnumMember();

            if (jsonElement.GetRequiredStringProperty(PropertyNames.Value, out var valueProperty))
            {
                enumMember.Value = valueProperty.GetString();
            }
            if (jsonElement.TryGetStringProperty(PropertyNames.Description, out var descriptionProperty))
            {
                enumMember.Description = descriptionProperty.GetString();
            }

            return enumMember;
        }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal void WriteTo(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(PropertyNames.Value, Value);
            jsonWriter.WriteStringOrNothing(PropertyNames.Description, Description);
            jsonWriter.WriteEndObject();
        }
    }
}