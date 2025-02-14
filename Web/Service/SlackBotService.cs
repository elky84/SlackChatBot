using System.Text;
using System.Text.Json;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;
using Web.Common.Config;
using File = System.IO.File;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Message = SlackNet.WebApi.Message;

namespace Web.Service;

public class SlackBotService : IEventHandler<MessageEvent>
{
    private readonly ILogger _log;
    private readonly string _historyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "conversation_history.json");

    private ISlackApiClient SlackApiClient { get; init; }
    private OllamaApiClient OllamaApiClient { get; init; }
    private SlackBotConfig SlackBotConfig { get; init; }

    private readonly List<OllamaSharp.Models.Chat.Message> _conversationHistory = [];

    public SlackBotService(ISlackApiClient slack, OllamaApiClient ollamaApiClient, ILogger<SlackBotService> log, SlackBotConfig slackBotConfig)
    {
        _log = log;
        
        SlackApiClient = slack;
        SlackBotConfig = slackBotConfig;
        OllamaApiClient = ollamaApiClient;
        
        // 대화 기록 로드
        LoadConversationHistory();

        // 기본 프롬프트 추가
        var promptsPath = Path.Combine(Directory.GetCurrentDirectory(), "Prompts");
        var files = Directory.GetFiles(promptsPath);
        foreach (var file in files)
        {
            var content = System.IO.File.ReadAllText(file);
            _conversationHistory.Add(new OllamaSharp.Models.Chat.Message(ChatRole.User, content));
        }
    }
    
    public async Task Handle(MessageEvent slackEvent)
    {
        if (string.IsNullOrEmpty(slackEvent.Text) || !string.IsNullOrEmpty(slackEvent.BotId))
            return;

        if (!slackEvent.Text.Contains(SlackBotConfig.Name))
            return;
        
        var message = slackEvent.Text.Replace(SlackBotConfig.Name, string.Empty);
        
        var keyValuePair = SlackBotConfig.Keywords.FirstOrDefault(x => slackEvent.Text.Contains(x.Key, StringComparison.OrdinalIgnoreCase));
        if(!string.IsNullOrEmpty(keyValuePair.Key))
        {
            await SlackApiClient.Chat.PostMessage(new Message
            {
                Text = SlackBotConfig.Catchphrase + " " + keyValuePair.Value,
                Channel = slackEvent.Channel,
                ThreadTs = slackEvent.ThreadTs ?? slackEvent.EventTs,
            });
        }
        else
        {
            var lastMessageTime = DateTime.UtcNow;
            var timeout = TimeSpan.FromSeconds(10);
            var responseBuilder = new StringBuilder();

            var chat = new Chat(OllamaApiClient)
            {
                Messages = _conversationHistory
            };

            await foreach (var answerToken in chat.SendAsync(message))
            {
                responseBuilder.Append(answerToken);
        
                // 10초가 경과했거나, 메시지가 끝났다면 전송
                if ((DateTime.UtcNow - lastMessageTime) >= timeout)
                {
                    await SendResponse(slackEvent, responseBuilder);
                    lastMessageTime = DateTime.UtcNow;
                }
            }

            await SendResponse(slackEvent, responseBuilder);

            // 대화 기록 업데이트 & 저장
            _conversationHistory.Add(new OllamaSharp.Models.Chat.Message(ChatRole.User, message));
            _conversationHistory.Add(new OllamaSharp.Models.Chat.Message(ChatRole.Assistant, responseBuilder.ToString()));
            SaveConversationHistory();
        }
    }
    
    async Task SendResponse(MessageEvent slackEvent, StringBuilder stringBuilder)
    {
        string currentText = stringBuilder.ToString();
        int lastPeriodIndex = currentText.LastIndexOfAny(['.', '!', '?']);

        if (lastPeriodIndex != -1)
        {
            var lastCompleteSentence = currentText[..(lastPeriodIndex + 1)];
            stringBuilder.Remove(0, lastPeriodIndex + 1);

            await SlackApiClient.Chat.PostMessage(new Message
            {
                Text = SlackBotConfig.Catchphrase + " " + lastCompleteSentence,
                Channel = slackEvent.Channel,
                ThreadTs = slackEvent.ThreadTs ?? slackEvent.EventTs,
            });
        }
    }

    private void SaveConversationHistory()
    {
        try
        {
            var json = JsonSerializer.Serialize(_conversationHistory, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_historyFilePath, json);
            _log.LogInformation("대화 기록이 저장되었습니다.");
        }
        catch (Exception ex)
        {
            _log.LogError($"대화 기록 저장 실패: {ex.Message}");
        }
    }

    private void LoadConversationHistory()
    {
        try
        {
            if (File.Exists(_historyFilePath))
            {
                var json = File.ReadAllText(_historyFilePath);
                var history = JsonSerializer.Deserialize<List<OllamaSharp.Models.Chat.Message>>(json);
                if (history != null)
                {
                    _conversationHistory.AddRange(history);
                    _log.LogInformation("대화 기록이 로드되었습니다.");
                }
            }
        }
        catch (Exception ex)
        {
            _log.LogError($"대화 기록 로드 실패: {ex.Message}");
        }
    }
}
