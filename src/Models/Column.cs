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
    /// A code list column.
    /// </summary>
    public abstract class Column
    {
        /// <summary>
        /// A human-readable description of the code list column.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The ID of the code list column.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the code list column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A boolean that specifies whether thecolumn value can be `null`.
        /// </summary>
        public bool? Nullable { get; set; }

        /// <summary>
        /// A boolean that defines whether this column is optional, i.e. whether it can be completely 
        /// omitted from a data row.
        /// </summary>
        public bool? Optional { get; set; }

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal abstract void WriteTo(Utf8JsonWriter jsonWriter);
    }
}