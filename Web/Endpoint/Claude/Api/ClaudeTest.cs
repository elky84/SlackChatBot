using Microsoft.AspNetCore.Authorization;
using Web.Common.Config;
using Web.Endpoint.Claude.Dto;
using Claudia;

namespace Web.Endpoint.Claude.Api;

public static class ClaudeTest
{
    [AllowAnonymous]
    public static async Task<ClaudeTestRes> Handle(ClaudeSettings claudeSettings, 
        ClaudeTestReq claudeTestReq, HttpRequest request)
    {

        var anthropic = new Anthropic
        {
            ApiKey = claudeSettings.ApiKey
        };

        var message = await anthropic.Messages.CreateAsync(new()
        {
            Model = "claude-3-5-sonnet-20240620",
            MaxTokens = 1024,
            Messages = [new() { Role = "user", Content = claudeTestReq.Message }]
        });

        Console.WriteLine(message);
        
        return new ClaudeTestRes
        {
            Message = message.Content.ToString()
        };
    }
}