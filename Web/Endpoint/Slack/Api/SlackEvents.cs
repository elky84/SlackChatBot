using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Web.Common.Config;

namespace Web.Endpoint.Slack.Api;

public static class SlackEvents
{
    [AllowAnonymous]
    public static async Task<IResult> Handle(SlackBotConfig slackBotConfig, HttpRequest request)
    {
        var slackEvent = await request.ReadFromJsonAsync<JObject>();
        if (slackEvent == null)
        {
            return Results.BadRequest();
        }

        // Slack 이벤트가 challenge인 경우
        if (slackEvent.ContainsKey("challenge"))
        {
            return Results.Ok(new
            {
                challenge = slackEvent["challenge"]
            });
        }

        // 메시지를 받았을 때 처리
        if (!slackEvent.TryGetValue("event", out var slackMessage))
            return Results.Empty;
        
        var userMessage = slackMessage["text"]?.ToString();
        var channelId = slackMessage["channel"]?.ToString();

        if (string.IsNullOrEmpty(userMessage) || string.IsNullOrEmpty(channelId))
        {
            return Results.Empty;
        }
            
        var response = GetResponseFromKeyword(userMessage, slackBotConfig.Keywords);
        await SendMessageToSlack(channelId, response);

        return Results.Ok();
    }

    static string GetResponseFromKeyword(string userMessage, Dictionary<string, string> keywords)
    {
        // 키워드를 찾고 해당하는 답변을 반환
        foreach (var keyword in keywords)
        {
            if (userMessage.Contains(keyword.Key))  // 키워드가 포함된 경우
            {
                return keyword.Value;
            }
        }

        // 기본 답변
        return "죄송합니다. 이해할 수 없는 질문입니다.";
    }

    static async Task SendMessageToSlack(string channelId, string message)
    {
        var client = new HttpClient();
        var payload = new
        {
            channel = channelId,
            text = message
        };

        client.DefaultRequestHeaders.Add("Authorization", "Bearer your-slack-bot-token");

        await client.PostAsJsonAsync("https://slack.com/api/chat.postMessage", payload);
    }
}