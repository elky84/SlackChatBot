namespace Web.Common.Config;

public record HuggingFaceSettings
{
    public string ApiKey { get; init; } = string.Empty;
}