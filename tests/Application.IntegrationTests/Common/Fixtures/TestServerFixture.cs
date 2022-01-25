using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.WebUI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.Common.Fixtures;

public class TestServerFixture : WebApplicationFactory<Startup>, IAsyncLifetime
{
    public DatabaseFixture DatabaseFixture { get; }
    public string? ConnectionString { get; set; }
    public readonly Mock<ICurrentUserService> MockCurrentUserService = new(MockBehavior.Strict);

    public TestServerFixture()
    {
        DatabaseFixture = new DatabaseFixture();
    }
    
    public void VerifyAllMocks() => Mock.VerifyAll(this.MockCurrentUserService);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped(_ => MockCurrentUserService.Object);
        });
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            Dictionary<string, string> data = new();
            if (ConnectionString != null)
            {
                data.Add("ConnectionStrings:DefaultConnection", ConnectionString);
            }

            configBuilder.AddInMemoryCollection(data);
        });
    }

    public async Task InitializeAsync()
    {
        await DatabaseFixture.InitializeAsync();
        ConnectionString = DatabaseFixture.Container.ConnectionString;

        using var scope = Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices
            .GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        VerifyAllMocks();
        await DatabaseFixture.DisposeAsync();
    }
}