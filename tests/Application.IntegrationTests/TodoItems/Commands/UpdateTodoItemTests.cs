using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoItems.Commands;

[Collection(nameof(ApiTestCollection))]
public class UpdateTodoItemTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public UpdateTodoItemTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = Guid.NewGuid(), Title = "New Title" };
        await FluentActions.Invoking(() => _fixture.SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ShouldUpdateTodoItem()
    {
        var userId = await _fixture.RunAsDefaultUserAsync();

        var listId = await _fixture.SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await _fixture.SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        var command = new UpdateTodoItemCommand
        {
            Id = itemId,
            Title = "Updated Item Title"
        };

        await _fixture.SendAsync(command);

        var item = await _fixture.FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.Title.Should().Be(command.Title);
        item.LastModifiedBy.Should().NotBeNull();
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().NotBeNull();
        item.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(10000));
    }
}
