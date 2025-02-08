using System.Text;
using OllamaSharp;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;
using Web.Common.Config;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Web.Service;

public class SlackBotService(ISlackApiClient slack, ILogger<SlackBotService> log, SlackBotConfig slackBotConfig) : IEventHandler<MessageEvent>
{
    private readonly ILogger _log = log;

    private OllamaApiClient? Ollama { get; set; }

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
            if (Ollama == null)
            {
                var uri = new Uri("http://localhost:11434");
                Ollama = new OllamaApiClient(uri);
                Ollama.SelectedModel = "llama3.3";
            }

            var lastMessageTime = DateTime.UtcNow;
            var timeout = TimeSpan.FromSeconds(10);
            var responseBuilder = new StringBuilder();

            var chat = new Chat(Ollama);
            await foreach (var answerToken in chat.SendAsync(slackEvent.Text))
            {
                responseBuilder.Append(answerToken);
        
                // 10초가 경과했거나, 메시지가 끝났다면 전송
                if ((DateTime.UtcNow - lastMessageTime) >= timeout)
                {
                    await SendResponse(slackEvent, responseBuilder);
                    lastMessageTime = DateTime.UtcNow; // 시간 초기화
                }
            }
    
            await SendResponse(slackEvent, responseBuilder); // 남은 응답 전송
        }
    }
    
    // Slack으로 응답 전송 (완성된 문장까지만)
    async Task SendResponse(MessageEvent slackEvent, StringBuilder stringBuilder)
    {
        string currentText = stringBuilder.ToString();
        int lastPeriodIndex = currentText.LastIndexOfAny(new[] { '.', '!', '?' });

        if (lastPeriodIndex != -1)
        {
            var lastCompleteSentence = currentText.Substring(0, lastPeriodIndex + 1);
            stringBuilder.Remove(0, lastPeriodIndex + 1); // 응답한 부분 제거

            await slack.Chat.PostMessage(new Message
            {
                Text = slackBotConfig.Catchphrase + " " + lastCompleteSentence,
                Channel = slackEvent.Channel,
                ThreadTs = slackEvent.ThreadTs ?? slackEvent.EventTs,
            });
        }
    }
}