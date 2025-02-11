using ChatGPT.Net.DTO.ChatGPT;
using Microsoft.AspNetCore.Authorization;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using Web.Common.Config;
using Web.Endpoint.Ollama.Dto;

namespace Web.Endpoint.Ollama.Api;

public static class OllamaTest
{
    private static OllamaApiClient? Ollama { get; set; }

    [AllowAnonymous]
    public static async Task<OllamaTestRes> Handle(OllamaTestReq ollamaTestReq, HttpRequest request)
    {
        if (Ollama == null)
        {
            var uri = new Uri("http://localhost:11434");
            Ollama = new OllamaApiClient(uri);
            Ollama.SelectedModel = "mistral:7b-instruct-q4_0";
        }

        var responseMessage = "";
        var chat = new Chat(Ollama);
        await foreach (var answerToken in chat.SendAsync(ollamaTestReq.Message))
        {
            responseMessage += answerToken;
        }

        return new OllamaTestRes
        {
            Message = responseMessage
        };
    }
}