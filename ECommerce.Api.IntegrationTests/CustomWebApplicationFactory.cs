using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:DefaultConnection",
            "Server=.;Database=Stores_Test;Trusted_Connection=True;TrustServerCertificate=True;");
        builder.UseSetting("ConnectionStrings:Redis", "localhost:6379");
        builder.UseSetting("Jwt:Secret", "test-secret-test-secret-test-secret-123456");
    }
}