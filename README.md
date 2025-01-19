# SlackChatBot

## 사용 패키지
- https://github.com/soxtoby/SlackNet

## SlackAPI 연동 참고 페이지
### AspNetCore
- https://github.com/soxtoby/SlackNet/blob/master/Examples/AspNetCoreExample/readme.md

# 로컬 서버 테스트용 LocalTunnel 사용 방법

## LocalTunnel 설치

`npm install -g localtunnel`

## LocalTunnel 실행

로컬 서버가 http://localhost:5000에서 실행되고 있다고 가정했을 때, 아래 명령어로 로컬 서버를 외부에서 접근 가능한 URL로 포워딩할 수 있습니다:

`lt --port 5000`
이 명령어를 실행하면 localtunnel은 임시 공개 URL을 제공합니다. 
예를 들어, https://abcdefg.loca.lt와 같은 URL이 출력됩니다.

## Slack Event Subscriptions 설정

Slack의 Event Subscriptions 페이지로 가서, Request URL에 localtunnel에서 제공한 URL을 입력합니다. 예를 들어, https://abcdefg.loca.lt/slack/events와 같이 입력합니다.

## 구독된 이벤트 구현 예시

```csharp
public class SlackBotService(ISlackApiClient slack, ILogger<SlackBotService> log) : IEventHandler<MessageEvent>
{
    private readonly ILogger _log = log;

    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text?.Contains("ping", StringComparison.OrdinalIgnoreCase) == true)
        {
            _log.LogInformation("Received ping from {User} in the {Channel} channel", (await slack.Users.Info(slackEvent.User)).Name, (await slack.Conversations.Info(slackEvent.Channel)).Name);
            
            await slack.Chat.PostMessage(new Message
            {
                Text = "pong",
                Channel = slackEvent.Channel
            });
        }
    }
}
```