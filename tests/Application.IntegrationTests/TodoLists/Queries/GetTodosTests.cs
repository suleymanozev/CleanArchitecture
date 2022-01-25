using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Queries;

[Collection(nameof(ApiTestCollection))]
public class GetTodosTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public GetTodosTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldReturnPriorityLevels()
    {
        var query = new GetTodosQuery();

        var result = await _fixture.SendAsync(query);

        result.PriorityLevels.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnAllListsAndItems()
    {
        await _fixture.AddAsync(new TodoList
        {
            Title = "Shopping",
            Colour = Colour.Blue,
            Items =
                    {
                        new TodoItem { Title = "Apples", Done = true },
                        new TodoItem { Title = "Milk", Done = true },
                        new TodoItem { Title = "Bread", Done = true },
                        new TodoItem { Title = "Toilet paper" },
                        new TodoItem { Title = "Pasta" },
                        new TodoItem { Title = "Tissues" },
                        new TodoItem { Title = "Tuna" }
                    }
        });

        var query = new GetTodosQuery();

        var result = await _fixture.SendAsync(query);

        result.Lists.Should().HaveCount(1);
        result.Lists.First().Items.Should().HaveCount(7);
    }
}
