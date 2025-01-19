using System.Text.Json;
using Web.Components;
using Web.Components.Http;
using Web.Domain.Account;
using Web.Domain.Char;
using Web.Endpoint;
using Microsoft.AspNetCore.Components.Server.Circuits;
using SlackNet.AspNetCore;
using SlackNet.Events;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Web.Common.Config;
using Web.Endpoint.Slack;
using Web.Service;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

builder.Configuration
    .AddJsonFile("appsettings.json", false, false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, false)
    .AddEnvironmentVariables();

var slackBotConfig = builder.Configuration.GetSection("SlackBot").Get<SlackBotConfig>();
var slackSettings = builder.Configuration.GetSection("Slack").Get<SlackSettings>()!;

builder.Services.AddHealthChecks();

#region Razor

services.AddRazorComponents(options => options.DetailedErrors = true)
    .AddInteractiveServerComponents(options => options.DetailedErrors = true)
    .AddCircuitOptions(options => options.DetailedErrors = true)
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);

#endregion // Razor

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

#region CORS

services.AddCors(options =>
    options.AddDefaultPolicy(corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

#endregion // CORS

#region Controllers

services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; })
    .ConfigureApiBehaviorOptions(options =>
    {
        // ASP.NET Core API Model Validation 예외를 400 Bad Request로 처리하지 않음. GlobalExceptionFilterHandler 에서 처리하도록 함
        options.SuppressModelStateInvalidFilter = true;
    });

#endregion // Controllers

#region Repository

services.AddSingleton<AccountRepository>();
services.AddSingleton<CharRepository>();

#endregion // Repository

#region Services

if(slackBotConfig != null)
    services.AddSingleton(slackBotConfig);

services.AddAntDesign().AddHttpContextAccessor();
services.AddHttpClientInterceptor();
services.AddScoped<HttpInterceptorService>();

services.AddScoped<CircuitHandler, CustomCircuitHandler>();

#region SlackNet

builder.Services.AddSlackNet(c => c
    // Configure the tokens used to authenticate with Slack
    .UseApiToken(slackSettings.ApiToken) // This gets used by the API client
    .UseAppLevelToken(slackSettings.AppLevelToken) // (Optional) used for socket mode
    // The signing secret ensures that SlackNet only handles requests from Slack 
    .UseSigningSecret(slackSettings.SigningSecret)
    
    // Register your Slack handlers here
    .RegisterEventHandler<MessageEvent, SlackBotService>()
);

#endregion // SlackNet

#endregion // Services

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/error", ExceptionController.Handler);
app.UseExceptionHandler("/error");

app.MapHealthChecks("/healthz");

#region Swagger

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#endregion // Swagger

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
});

app.MapControllers()
    .RequireAuthorization();

// FYI: EnvironmentName=Local 인 경우 css 등 정적 리소스를 찾지 못하는 문제 수정.
// https://forum.radzen.com/t/radzen-Web-components-dont-respect-custom-environments/2690/4
// Radzen 해결 책이지만 AntDesign.Web에서도 같은 해결책이 적용
builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

#region api

var api = app.MapGroup("/api");

SlackEndpoint.Map(api);

#endregion api

#region SlackNet

app.UseSlackNet();

#endregion // SlackNet

await app.RunAsync();

#pragma warning disable S1118
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program // for UnitTest
{
}
#pragma warning restore S1118