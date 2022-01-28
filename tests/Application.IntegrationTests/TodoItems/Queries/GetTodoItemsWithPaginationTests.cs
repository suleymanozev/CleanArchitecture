using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoItems.Queries;

[Collection(nameof(ApiTestCollection))]
public class GetTodoItemsWithPaginationTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public GetTodoItemsWithPaginationTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new GetTodoItemsWithPaginationQuery();
        await FluentActions.Invoking(() => _fixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ShouldGetTodoItemsWithPagination()
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

        var query = new GetTodoItemsWithPaginationQuery {ListId = list.Id, PageNumber = 1, PageSize = 10};

        var result = await _fixture.SendAsync(query);
        result.Items.Should().HaveCount(list.Items.Count);
        result.Items.First(x => x.Title!.Equals("Apples")).Id.Should().NotBeEmpty();
        result.Items.First(x => x.Title!.Equals("Apples")).Title.Should().Be("Apples");
        result.Items.First(x => x.Title!.Equals("Apples")).ListId.Should().Be(list.Id);
        result.Items.First(x => x.Title!.Equals("Apples")).Done.Should().Be(true);
        result.PageNumber.Should().Be(query.PageNumber);
        result.TotalPages.Should().Be((int)Math.Ceiling((double)list.Items.Count / query.PageSize));
        result.TotalCount.Should().Be(list.Items.Count);
        result.HasNextPage.Should().Be(false);
        result.HasPreviousPage.Should().Be(false);
    }
}