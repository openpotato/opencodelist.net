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

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenCodeList;

/// <summary>
/// An external code list reference.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = PropertyNames.Type)]
[JsonDerivedType(typeof(ExternalCodeListDocumentRef), TypeConsts.CodeListRef)]
[JsonDerivedType(typeof(ExternalCodeListSetDocumentRef), TypeConsts.CodeListSetRef)]
public abstract class ExternalCodeListBaseRef
{
    /// <summary>
    /// Annotations for the document.
    /// </summary>
    [JsonPropertyName(PropertyNames.Annotation)]
    [JsonPropertyOrder(10)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Annotation Annotation { get; set; }

    /// <summary>
    /// Canonical URI which uniquely identifies all versions (collectively)
    /// </summary>
    [JsonPropertyName(PropertyNames.CanonicalUri)]
    [JsonPropertyOrder(11)]
    [JsonRequired]
    public Uri CanonicalUri { get; set; }

    /// <summary>
    /// Canonical URI which uniquely identifies this version.
    /// </summary>
    [JsonPropertyName(PropertyNames.CanonicalVersionUri)]
    [JsonPropertyOrder(12)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri CanonicalVersionUri { get; set; }

    /// <summary>
    /// Suggested retrieval location for this version, in OpenCodeList format.
    /// </summary>
    [JsonPropertyName(PropertyNames.LocationUrls)]
    [JsonPropertyOrder(13)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Uri> LocationUrls { get; set; } = [];
}