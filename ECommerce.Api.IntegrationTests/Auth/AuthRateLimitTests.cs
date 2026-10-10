using System.Net;
using System.Net.Http.Json;

namespace ECommerce.Api.IntegrationTests.Auth;

public class AuthRateLimitTests:IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;
    public AuthRateLimitTests(CustomWebApplicationFactory factory) =>
        client = factory.CreateClient();
    [Fact]
    public async Task LogenIsRateLimitedAfter10Requests()
    {
        HttpResponseMessage? last = null;
        for (int i = 0; i < 11; i++)
        {
            last =await client.PostAsJsonAsync("/api/v1/auth/login",
                new { email = "nobody@test.com", password = "Wrong123" });
        }
        Assert.Equal(HttpStatusCode.TooManyRequests, last!.StatusCode);
    }
}
