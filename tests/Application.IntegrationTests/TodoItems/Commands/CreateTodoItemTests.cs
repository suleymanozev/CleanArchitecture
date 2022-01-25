using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoItems.Commands;

[Collection(nameof(ApiTestCollection))]
public class CreateTodoItemTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public CreateTodoItemTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoItemCommand();

        await FluentActions.Invoking(() =>
            _fixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ShouldCreateTodoItem()
    {
        var userId = await _fixture.RunAsDefaultUserAsync();

        var listId = await _fixture.SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var command = new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tasks"
        };

        var itemId = await _fixture.SendAsync(command);

        var item = await _fixture.FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.ListId.Should().Be(command.ListId);
        item.Title.Should().Be(command.Title);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.Should().BeNull();
        item.LastModified.Should().BeNull();
    }
}
