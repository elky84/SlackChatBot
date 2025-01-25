using Web.Endpoint.HuggingFace.Api;

namespace Web.Endpoint.HuggingFace;

public static class HuggingFaceEndpoint
{
    public static void Map(RouteGroupBuilder routeGroup)
    {
        var api = routeGroup.MapGroup("HuggingFace")
            .WithTags(nameof(HuggingFace));

        api.MapPost("/test", HuggingFaceTest.Handle);
    }
}
