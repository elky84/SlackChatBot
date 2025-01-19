using static System.DateTime;

namespace Web.Common;

public record BaseResponse
{
    public int StatusCode { get; set; } = 200;

    public string? Message { get; set; } = "success";

    public long CurrentUtcTimeStamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public long CurrentLocalTimeStamp { get; set; } = (Now - UnixEpoch).Ticks / TimeSpan.TicksPerSecond;
}