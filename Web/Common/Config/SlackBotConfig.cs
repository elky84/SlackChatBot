namespace Web.Common.Config;

public class SlackBotConfig
{
    public string ApiToken { get; set; } = string.Empty;
    
    public Dictionary<string, string> Keywords { get; set; } = [];
}