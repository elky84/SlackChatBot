using Web.Endpoint.Claude.Api;

namespace Web.Endpoint.Claude;

public static class ClaudeEndpoint
{
    public static void Map(RouteGroupBuilder routeGroup)
    {
        var api = routeGroup.MapGroup("Claude")
            .WithTags(nameof(Claude));

        api.MapPost("/test", ClaudeTest.Handle);
    }
}
