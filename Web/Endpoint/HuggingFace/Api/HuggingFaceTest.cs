using Microsoft.AspNetCore.Authorization;
using System.Text;
using Web.Common.Config;
using Web.Endpoint.HuggingFace.Dto;

namespace Web.Endpoint.HuggingFace.Api;

public static class HuggingFaceTest
{
    [AllowAnonymous]
    public static async Task<HuggingFaceTestRes> Handle(HuggingFaceSettings huggingFaceSettings, 
        HuggingFaceTestReq huggingFaceTestReq, HttpRequest request)
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(5)
        };
        const string url = "https://api-inference.huggingface.co/models/EleutherAI/gpt-neo-1.3B";

        var apiKey = huggingFaceSettings.ApiKey;

        var jsonData = new
        {
            inputs = huggingFaceTestReq.Message
        };

        var content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(jsonData),
            Encoding.UTF8, "application/json");

        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var response = await client.PostAsync(url, content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return new HuggingFaceTestRes
            {
                Message = responseBody
            };
        }
        else
        {
            return new HuggingFaceTestRes
            {
                Message = "API 호출 실패: " + response.StatusCode + " ResponseContent: " + response.Content.ReadAsStringAsync(),
            };        
        }
    }
}