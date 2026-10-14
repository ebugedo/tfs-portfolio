// <copyright file="HttpClientExtensions.cs" company="Tfs.Portfolio">
// Copyright (c) Tfs.Portfolio. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tfs.Portfolio.IntegrationTests.Common;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tfs.Portfolio.Api.Json;

/// <summary>
/// Extension methods for HttpClient using Newtonsoft.Json for serialization.
/// </summary>
[SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "HttpClient takes ownership of StringContent and disposes it")]
public static class HttpClientExtensions
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new TfsWebUrlContractResolver()
    };

    private static readonly JsonSerializerOptions SystemTextJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Sends a POST request with JSON content using Newtonsoft.Json.
    /// </summary>
    public static async Task<HttpResponseMessage> PostAsJsonNewtonsoftAsync<T>(this HttpClient client, string requestUri, T value)
        => await PostAsJsonNewtonsoftAsync(client, new Uri(requestUri, UriKind.RelativeOrAbsolute), value);

    /// <summary>
    /// Sends a POST request with JSON content using Newtonsoft.Json.
    /// </summary>
    [SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "HttpClient takes ownership of StringContent and disposes it")]
    public static async Task<HttpResponseMessage> PostAsJsonNewtonsoftAsync<T>(this HttpClient client, Uri requestUri, T value)
    {
        var json = JsonConvert.SerializeObject(value, JsonSettings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(requestUri, content);
    }

    /// <summary>
    /// Sends a PUT request with JSON content using Newtonsoft.Json.
    /// </summary>
    public static async Task<HttpResponseMessage> PutAsJsonNewtonsoftAsync<T>(this HttpClient client, string requestUri, T value)
        => await PutAsJsonNewtonsoftAsync(client, new Uri(requestUri, UriKind.RelativeOrAbsolute), value);

    /// <summary>
    /// Sends a PUT request with JSON content using Newtonsoft.Json.
    /// </summary>
    [SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "HttpClient takes ownership of StringContent and disposes it")]
    public static async Task<HttpResponseMessage> PutAsJsonNewtonsoftAsync<T>(this HttpClient client, Uri requestUri, T value)
    {
        var json = JsonConvert.SerializeObject(value, JsonSettings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PutAsync(requestUri, content);
    }

    /// <summary>
    /// Sends a PATCH request with JSON content using Newtonsoft.Json.
    /// </summary>
    public static async Task<HttpResponseMessage> PatchAsJsonNewtonsoftAsync<T>(this HttpClient client, string requestUri, T value)
        => await PatchAsJsonNewtonsoftAsync(client, new Uri(requestUri, UriKind.RelativeOrAbsolute), value);

    /// <summary>
    /// Sends a PATCH request with JSON content using Newtonsoft.Json.
    /// </summary>
    [SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "HttpClient takes ownership of StringContent and disposes it")]
    public static async Task<HttpResponseMessage> PatchAsJsonNewtonsoftAsync<T>(this HttpClient client, Uri requestUri, T value)
    {
        var json = JsonConvert.SerializeObject(value, JsonSettings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PatchAsync(requestUri, content);
    }

    /// <summary>
    /// Reads the response content as JSON using Newtonsoft.Json.
    /// </summary>
    public static async Task<T?> ReadFromJsonNewtonsoftAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }
        return JsonConvert.DeserializeObject<T>(json, JsonSettings);
    }

    /// <summary>
    /// Reads the response content as JSON using System.Text.Json (for minimal APIs that use System.Text.Json).
    /// </summary>
    public static async Task<T?> ReadFromJsonAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, SystemTextJsonOptions);
    }
}
