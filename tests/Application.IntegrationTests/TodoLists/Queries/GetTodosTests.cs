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
        result.PriorityLevels.First().Name.Should().NotBeNullOrEmpty();
        result.PriorityLevels.First().Value.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task ShouldReturnAllListsAndItems()
    {
        var list = new TodoList
        {
            Title = "Shopping",
            Colour = Colour.Blue,
            Items =
            {
                new TodoItem {Title = "Apples", Done = true},
                new TodoItem {Title = "Milk", Done = true},
                new TodoItem {Title = "Bread", Done = true},
                new TodoItem {Title = "Toilet paper"},
                new TodoItem {Title = "Pasta"},
                new TodoItem {Title = "Tissues"},
                new TodoItem {Title = "Tuna"}
            }
        };
        await _fixture.AddAsync(list);

        var query = new GetTodosQuery();

        var result = await _fixture.SendAsync(query);

        result.Lists.Should().HaveCount(1);
        result.Lists.First().Id.Should().Be(list.Id);
        result.Lists.First().Title.Should().Be(list.Title);
        result.Lists.First().Colour.Should().Be(list.Colour);
        result.Lists.First().Items.Should().HaveCount(7);
        result.Lists.First().Items.First(x => x.Title!.Equals("Apples")).Id.Should().NotBeEmpty();
        result.Lists.First().Items.First(x => x.Title!.Equals("Apples")).Title.Should().NotBeNullOrEmpty();
        result.Lists.First().Items.First(x => x.Title!.Equals("Apples")).Priority.Should().BeGreaterThanOrEqualTo(0);
        result.Lists.First().Items.First(x => x.Title!.Equals("Apples")).Done.Should().BeTrue();
        result.Lists.First().Items.First(x => x.Title!.Equals("Apples")).Note.Should().BeNull();
        result.Lists.First().Items.First(x => x.Title!.Equals("Apples")).ListId.Should().Be(list.Id);
    }
}