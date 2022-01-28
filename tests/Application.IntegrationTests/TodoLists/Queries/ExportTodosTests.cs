using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Queries;

[Collection(nameof(ApiTestCollection))]
public class ExportTodosTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public ExportTodosTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldExportTodos()
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
        
        
        var query = new ExportTodosQuery
        {
            ListId = list.Id
        };

        var result = await _fixture.SendAsync(query);
        result.FileName.Should().Be("TodoItems.csv");
        result.ContentType.Should().Be("text/csv");
        result.Content.Should().NotBeNullOrEmpty();
    }
}