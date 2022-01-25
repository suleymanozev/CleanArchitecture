using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Commands;

[Collection(nameof(ApiTestCollection))]
public class UpdateTodoListTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public UpdateTodoListTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new UpdateTodoListCommand {Id = Guid.NewGuid(), Title = "New Title"};
        await FluentActions.Invoking(() => _fixture.SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ShouldRequireUniqueTitle()
    {
        var listId = await _fixture.SendAsync(new CreateTodoListCommand {Title = "New List"});

        await _fixture.SendAsync(new CreateTodoListCommand {Title = "Other List"});

        var command = new UpdateTodoListCommand {Id = listId, Title = "Other List"};

        (await FluentActions.Invoking(() =>
                    _fixture.SendAsync(command))
                .Should().ThrowAsync<ValidationException>().Where(ex => ex.Errors.ContainsKey("Title")))
            .And.Errors["Title"].Should().Contain("The specified title already exists.");
    }

    [Fact]
    public async Task ShouldUpdateTodoList()
    {
        var userId = await _fixture.RunAsDefaultUserAsync();

        var listId = await _fixture.SendAsync(new CreateTodoListCommand {Title = "New List"});

        var command = new UpdateTodoListCommand {Id = listId, Title = "Updated List Title"};

        await _fixture.SendAsync(command);

        var list = await _fixture.FindAsync<TodoList>(listId);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list.LastModifiedBy.Should().NotBeNull();
        list.LastModifiedBy.Should().Be(userId);
        list.LastModified.Should().NotBeNull();
        list.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(10000));
    }
}