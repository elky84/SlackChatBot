namespace Web.Common.Config;

public record ChatGptSettings
{
    public string ApiKey { get; init; } = string.Empty;
}