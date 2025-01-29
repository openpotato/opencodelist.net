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
    /// An external code list reference.
    /// </summary>
    public abstract class DocumentRef
    {
        /// <summary>
        /// Canonical URI which uniquely identifies all versions (collectively)
        /// </summary>
        public Uri CanonicalUri { get; set; }

        /// <summary>
        /// Canonical URI which uniquely identifies this version.
        /// </summary>
        public Uri CanonicalVersionUri { get; set; }

        /// <summary>
        /// Suggested retrieval location for this version, in OpenCodeList format.
        /// </summary>
        public IList<Uri> LocationUrls { get; set; } = [];

        /// <summary>
        /// Writes the content as json object to a <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="Utf8JsonWriter"/> instance</param>
        internal abstract void WriteTo(Utf8JsonWriter jsonWriter);
    }
}