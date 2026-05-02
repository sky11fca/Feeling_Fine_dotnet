using DotnetApi.Infrastructure.Persistance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace DotnetApi.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var settings = new System.Collections.Generic.Dictionary<string, string?>
                {
                    {"JwtSettings:Secret", "A_Very_Long_And_Secure_Dummy_Secret_Key_For_Testing_12345"},
                    {"JwtSettings:Issuer", "TestIssuer"},
                    {"JwtSettings:Audience", "TestAudience"}
                };
                config.AddInMemoryCollection(settings);
            });

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                var redisDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache));

                if (redisDescriptor != null)
                {
                    services.Remove(redisDescriptor);
                }

                services.AddDistributedMemoryCache();
            });
        }
    }
}
