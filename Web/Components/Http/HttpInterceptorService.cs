using AntDesign;
using Toolbelt.Blazor;

namespace Web.Components.Http;

public class HttpInterceptorService(
    HttpClientInterceptor interceptor,
    ILogger<HttpInterceptorService> logger,
    ModalService modalService)
{
    public void RegisterEvent()
    {
        interceptor.AfterSendAsync += InterceptResponse;
    }

    private async Task InterceptResponse(object sender, HttpClientInterceptorEventArgs e)
    {
        if (!e.Response.IsSuccessStatusCode)
        {
            var capturedContent = await e.GetCapturedContentAsync();
            var content = await capturedContent.ReadFromJsonAsync<ErrorResponse>();
            if (content?.Errors.Length > 0)
            {
                logger.LogError("Response Error. {Message}", content.Errors[0].Message);
                await modalService.ErrorAsync(new ConfirmOptions
                {
                    Title = e.Response.StatusCode.ToString(),
                    Content = content.Errors[0].Message,
                    Style = "width: 80%; max-width: 800px;"
                });
            }
            else
            {
                await modalService.ErrorAsync(new ConfirmOptions
                {
                    Title = e.Response.StatusCode.ToString(),
                    Content = "Unknown Error",
                    Style = "width: 80%; max-width: 800px;"
                });
            }
        }
    }

    public void DisposeEvent()
    {
        interceptor.AfterSendAsync -= InterceptResponse;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class ExecutionError
    {
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string Message { get; set; } = string.Empty;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class ErrorResponse
    {
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public ExecutionError[] Errors { get; set; } = [];
    }
}