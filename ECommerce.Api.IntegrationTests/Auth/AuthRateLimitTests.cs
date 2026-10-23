using System.Net;
using System.Net.Http.Json;

namespace ECommerce.Api.IntegrationTests.Auth;

public class AuthRateLimitTests:IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;

    public AuthRateLimitTests(CustomWebApplicationFactory factory) =>
        client = factory.CreateClient();
    [Fact]
    public async Task Login_is_rate_limited_after_10_requests()
    {
        var body = new { Email = "invalid-email", Password = "" };

        // الـ 10 المسموح بيهم: الـ Validation بيرفضهم بـ 400 من غير ما يلمسوا الـ DB
        for (var i = 0; i < 10; i++)
        {
            var response = await client.PostAsJsonAsync("/api/v1/auth/login", body);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // الطلب رقم 11 هو اللي لازم يتقفل
        var limited = await client.PostAsJsonAsync("/api/v1/auth/login", body);
        Assert.Equal(HttpStatusCode.TooManyRequests, limited.StatusCode);
    }
}
