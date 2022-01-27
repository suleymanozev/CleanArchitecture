using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.MessageBrokers;
using DotNet.Testcontainers.Containers.Modules.MessageBrokers;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.Common.Fixtures;

public class QueueFixture : IAsyncLifetime
{
    public RabbitMqTestcontainer Container { get; }

    public QueueFixture()
    {
        Container = new TestcontainersBuilder<RabbitMqTestcontainer>()
            .WithMessageBroker(new RabbitMqTestcontainerConfiguration("rabbitmq:3-alpine")
            {
                Username = "guest",
                Password = "guest"
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