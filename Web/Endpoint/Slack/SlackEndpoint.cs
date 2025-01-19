using Web.Endpoint.Slack.Api;

namespace Web.Endpoint.Slack;

public static class SlackEndpoint
{
    public static void Map(RouteGroupBuilder routeGroup)
    {
        var api = routeGroup.MapGroup("Slack")
            .WithTags(nameof(Slack));

        api.MapPost("/events", SlackEvents.Handle);
        api.MapPost("/test", SlackTest.Handle);
    }
}
