using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.PurgeTodoLists;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Commands;

[Collection(nameof(ApiTestCollection))]
public class PurgeTodoListsTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public PurgeTodoListsTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldDenyAnonymousUser()
    {
        var command = new PurgeTodoListsCommand();

        command.GetType().Should().BeDecoratedWith<AuthorizeAttribute>();

        await FluentActions.Invoking(() =>
            _fixture.SendAsync(command)).Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ShouldDenyNonAdministrator()
    {
        await _fixture.RunAsDefaultUserAsync();

        var command = new PurgeTodoListsCommand();

        await FluentActions.Invoking(() =>
            _fixture.SendAsync(command)).Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task ShouldAllowAdministrator()
    {
        await _fixture.RunAsAdministratorAsync();

        var command = new PurgeTodoListsCommand();

        await FluentActions.Invoking(() => _fixture.SendAsync(command))
             .Should().NotThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task ShouldDeleteAllLists()
    {
        await _fixture.RunAsAdministratorAsync();

        await _fixture.SendAsync(new CreateTodoListCommand
        {
            Title = "New List #1"
        });

        await _fixture.SendAsync(new CreateTodoListCommand
        {
            Title = "New List #2"
        });

        await _fixture.SendAsync(new CreateTodoListCommand
        {
            Title = "New List #3"
        });

        await _fixture.SendAsync(new PurgeTodoListsCommand());

        var count = await _fixture.CountAsync<TodoList>();

        count.Should().Be(0);
    }
}
