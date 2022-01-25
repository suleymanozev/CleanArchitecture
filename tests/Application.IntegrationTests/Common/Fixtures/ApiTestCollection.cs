using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.Common.Fixtures;

[CollectionDefinition(nameof(ApiTestCollection))]
public class ApiTestCollection : ICollectionFixture<TestServerFixture>
{
    
}