using System.Net;

namespace ECommerce.Api.IntegrationTests.Products;

public class ProductAuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;
    public ProductAuthorizationTests(CustomWebApplicationFactory factory)
    {
        client=factory.CreateClient();
    }

    [Theory]
    [InlineData("POST", "/api/v1/products")]
    [InlineData("PUT", "/api/v1/products/3fa85f64-5717-4562-b3fc-2c963f66afa6")]
    [InlineData("DELETE", "/api/v1/products/3fa85f64-5717-4562-b3fc-2c963f66afa6")]
    [InlineData("POST", "/api/v1/attachments/cleanup")]
    public async Task Mutating_endpoints_without_token_return_401(string method, string url)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), url);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

}
