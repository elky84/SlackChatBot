using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.JSInterop;

namespace Web.Components.Http;

public class CustomCircuitHandler(IJSRuntime jsRuntime) : CircuitHandler
{
    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await jsRuntime.InvokeVoidAsync("location.reload", cancellationToken);

        await base.OnConnectionDownAsync(circuit, cancellationToken);
    }
}