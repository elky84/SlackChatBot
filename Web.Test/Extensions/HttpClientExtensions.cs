using System.Net.Http.Json;

namespace Web.Test.Extensions;

public static class HttpClientExtensions
{
    public static async Task<TResponse?> Post<TResponse>(this HttpClient httpClient, string requestUri, object value)
        where TResponse : class
    {
        using var response = await httpClient.PostAsJsonAsync(requestUri, value);
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }
}