using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.Common.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    public PostgreSqlTestcontainer Container { get; }

    public DatabaseFixture()
    {
        Container = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration("postgres:14-alpine")
            {
                Database = "db",
                Username = "postgres",
                Password = "postgres"
            })
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}