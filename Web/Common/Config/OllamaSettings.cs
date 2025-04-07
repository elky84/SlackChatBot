namespace Web.Common.Config;

public record OllamaSettings
{
    public string Uri { get; init; } = string.Empty;
    
    public string SelectedModel { get; init; } = string.Empty;
}