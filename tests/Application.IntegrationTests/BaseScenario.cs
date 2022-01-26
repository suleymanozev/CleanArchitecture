using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests;

public abstract class BaseScenario : IAsyncLifetime
{
    private readonly TestServerFixture _fixture;
    private readonly Checkpoint _checkpoint;
        
    protected BaseScenario(TestServerFixture fixture)
    {
        _fixture = fixture;
            
        _checkpoint = new Checkpoint
        {
            TablesToIgnore = new Table[] {"__EFMigrationsHistory"},
            SchemasToInclude = new[] {"public"},
            DbAdapter = DbAdapter.Postgres
        };
    }

    public async Task InitializeAsync()
    {
        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        await _checkpoint.Reset(conn);
        _fixture.MockCurrentUserService.SetupGet(s => s.UserId).Returns((Guid?)null);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}