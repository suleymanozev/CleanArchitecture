using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoItems.Commands;

[Collection(nameof(ApiTestCollection))]
public class DeleteTodoItemTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public DeleteTodoItemTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand { Id = Guid.NewGuid() };

        await FluentActions.Invoking(() =>
            _fixture.SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ShouldDeleteTodoItem()
    {
        await _fixture.RunAsDefaultUserAsync();
        var listId = await _fixture.SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await _fixture.SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        await _fixture.SendAsync(new DeleteTodoItemCommand
        {
            Id = itemId
        });

        var item = await _fixture.FindAsync<TodoItem>(itemId);

        item.Should().BeNull();
    }
}
