using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.DeleteTodoList;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Commands;

[Collection(nameof(ApiTestCollection))]
public class DeleteTodoListTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public DeleteTodoListTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new DeleteTodoListCommand { Id = Guid.NewGuid() };
        await FluentActions.Invoking(() => _fixture.SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ShouldDeleteTodoList()
    {
        await _fixture.RunAsDefaultUserAsync();
        var listId = await _fixture.SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await _fixture.SendAsync(new DeleteTodoListCommand
        {
            Id = listId
        });

        var list = await _fixture.FindAsync<TodoList>(listId);

        list.Should().BeNull();
    }
}
