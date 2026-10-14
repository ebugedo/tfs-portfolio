// <copyright file="TfsWebUrlContractResolver.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.Api.Json;

using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tfs.Portfolio.Domain.Common.ValueObjects;

/// <summary>
/// Custom contract resolver that applies the TfsWebUrlJsonConverter to all TfsWebUrl? properties.
/// </summary>
public sealed class TfsWebUrlContractResolver : CamelCasePropertyNamesContractResolver
{
    /// <inheritdoc />
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // Apply the TfsWebUrlJsonConverter to all TfsWebUrl? properties (both TfsWebUrl? and Nullable<TfsWebUrl>)
        Type? propertyType = property.PropertyType;
        if (propertyType == null)
        {
            return property;
        }

        Type? underlyingType = Nullable.GetUnderlyingType(propertyType);

        bool isNullableTfsWebUrl = propertyType == typeof(TfsWebUrl?) || (underlyingType != null && underlyingType == typeof(TfsWebUrl));

        if (isNullableTfsWebUrl)
        {
            property.Converter = new TfsWebUrlJsonConverter();
        }

        return property;
    }
}
