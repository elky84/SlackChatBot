using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace Web.Test;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class WebFixture : IClassFixture<WebFixture>, IDisposable, IAsyncDisposable
{
    public WebFixture()
    {
        var builder = WebApplication.CreateBuilder();
        Configuration = builder.Configuration
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var services = builder.Services;

        ServiceProvider = services.BuildServiceProvider();
        Logger = ServiceProvider.GetRequiredService<ILogger<WebFixture>>();
    }

    private ServiceProvider ServiceProvider { get; }
    public IConfiguration Configuration { get; }
    public ILogger<WebFixture> Logger { get; set; }

    public async ValueTask DisposeAsync()
    {
        await ServiceProvider.DisposeAsync();
    }

    public void Dispose()
    {
        ServiceProvider.Dispose();
    }

    public T GetRequiredService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    public void SetOutputHelper(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("MySqlConnector.MySqlCommand", LogEventLevel.Information)
            .WriteTo.TestOutput(output)
            .CreateLogger();

        Logger = ServiceProvider.GetRequiredService<ILogger<WebFixture>>();
    }
}