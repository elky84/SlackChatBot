using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;
using Web.Common.Config;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Web.Service;

public class SlackBotService(ISlackApiClient slack, ILogger<SlackBotService> log, SlackBotConfig slackBotConfig) : IEventHandler<MessageEvent>
{
    private readonly ILogger _log = log;

    public async Task Handle(MessageEvent slackEvent)
    {
        if (string.IsNullOrEmpty(slackEvent.Text) || !string.IsNullOrEmpty(slackEvent.BotId))
            return;

        if (!slackEvent.Text.Contains(slackBotConfig.Name))
            return;
        
        var keyValuePair = slackBotConfig.Keywords.FirstOrDefault(x => slackEvent.Text.Contains(x.Key, StringComparison.OrdinalIgnoreCase));
        if(!string.IsNullOrEmpty(keyValuePair.Key))
        {
            await slack.Chat.PostMessage(new Message
            {
                Text = slackBotConfig.Catchphrase + " " + keyValuePair.Value,
                Channel = slackEvent.Channel,
                ThreadTs = slackEvent.ThreadTs ?? slackEvent.EventTs,
            });
        }
        else
        {
            await slack.Chat.PostMessage(new Message
            {
                Text = slackBotConfig.Catchphrase + " 못알아들었다!",
                Channel = slackEvent.Channel,
                ThreadTs = slackEvent.ThreadTs ?? slackEvent.EventTs,
            });
        }
    }
}