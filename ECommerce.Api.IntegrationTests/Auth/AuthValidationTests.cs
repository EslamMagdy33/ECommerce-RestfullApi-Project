
using System.Net;
using System.Net.Http.Json;

namespace ECommerce.Api.IntegrationTests.Auth
{
    public class AuthValidationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient client;
        public AuthValidationTests(CustomWebApplicationFactory factory) =>
            client = factory.CreateClient();
        [Fact]
        public async Task LoginWithInvalidBodyReturnsBadRequestNot500()
        {
            var invalidLoginRequest = new
            {
                Email = "invalid-email",
                Password = ""
            };
            var response = await client.PostAsJsonAsync("/api/v1/auth/login", invalidLoginRequest);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }
        [Fact]
        public async Task Register_with_invalid_body_returns_400_not_500()
        {
            var response = await client.PostAsJsonAsync("/api/v1/auth/register",
                new { email = "not-an-email", password = "1", displayName = "" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

