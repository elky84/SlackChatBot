namespace Web.Common.Config;

public class SlackBotConfig
{
    public string ApiToken { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Call { get; set; } = string.Empty;

    public string Catchphrase { get; set; } = string.Empty;
    
    public Dictionary<string, string> Keywords { get; set; } = [];
}