using ChatGPT.Net.DTO.ChatGPT;
using Microsoft.AspNetCore.Authorization;
using Web.Common.Config;
using Web.Endpoint.ChatGpt.Dto;

namespace Web.Endpoint.ChatGpt.Api;

public static class ChatGptTest
{
    [AllowAnonymous]
    public static async Task<ChatGptTestRes> Handle(ChatGptSettings chatGptSettings, 
        ChatGptTestReq chatGptTestReq, HttpRequest request)
    {
        var bot = new ChatGPT.Net.ChatGpt(chatGptSettings.ApiKey, new ChatGptOptions
        {
            Model = "gpt-3.5-turbo"
        });

        var response = await bot.Ask(chatGptTestReq.Message);
        Console.WriteLine(response);
        return new ChatGptTestRes
        {
            Message = response
        };
    }
}