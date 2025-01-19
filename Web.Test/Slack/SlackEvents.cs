using Xunit.Abstractions;

namespace Web.Test.Slack;

[Collection(nameof(Web))]
public class SlackEvents: IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly WebFixture _fixture;

    public SlackEvents(WebFixture fixture, CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.SetOutputHelper(output);

        _factory = factory;
    }

    // [Fact]
    // public async Task EmptyAccount()
    // {
    //     using var client = _factory.CreateClient();
    //
    //     var resGuestSignIn = await client.Post<GuestSignInRes>("/api/account/GuestSignIn",
    //         new GuestSignInReq("", "", ""));
    //
    //     Assert.NotNull(resGuestSignIn);
    //
    //     Assert.Equal(500, resGuestSignIn.StatusCode);
    // }
}