using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SlackNet;
using SlackNet.WebApi;
using Web.Common.Config;
using Web.Service;

namespace Web.Endpoint.Slack.Api;

public static class SlackTest
{
    [AllowAnonymous]
    public static async Task<IResult> Handle(ISlackApiClient apiClient, HttpRequest request)
    {
        await apiClient.Chat.PostMessage(new Message
        {
            Text = "Hello, Slack!",
            Channel = "C089PBFN4C9"
        });

        return Results.Ok();
    }
}