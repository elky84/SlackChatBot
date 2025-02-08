using Web.Endpoint.Ollama.Api;

namespace Web.Endpoint.Ollama;

public static class OllamaEndpoint
{
    public static void Map(RouteGroupBuilder routeGroup)
    {
        var api = routeGroup.MapGroup("Ollama")
            .WithTags(nameof(Ollama));

        api.MapPost("/test", OllamaTest.Handle);
    }
}
