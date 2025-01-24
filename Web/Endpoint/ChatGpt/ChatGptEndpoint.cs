using Web.Endpoint.ChatGpt.Api;

namespace Web.Endpoint.ChatGpt;

public static class ChatGptEndpoint
{
    public static void Map(RouteGroupBuilder routeGroup)
    {
        var api = routeGroup.MapGroup("ChatGpt")
            .WithTags(nameof(ChatGpt));

        api.MapPost("/test", ChatGptTest.Handle);
    }
}
