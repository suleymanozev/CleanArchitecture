using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Commands;

[Collection(nameof(ApiTestCollection))]
public class CreateTodoListTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public CreateTodoListTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoListCommand();
        await FluentActions.Invoking(() => _fixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ShouldRequireUniqueTitle()
    {
        await _fixture.SendAsync(new CreateTodoListCommand
        {
            Title = "Shopping"
        });

        var command = new CreateTodoListCommand
        {
            Title = "Shopping"
        };

        await FluentActions.Invoking(() =>
            _fixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ShouldCreateTodoList()
    {
        var userId = await _fixture.RunAsDefaultUserAsync();

        var command = new CreateTodoListCommand
        {
            Title = "Tasks"
        };

        var id = await _fixture.SendAsync(command);

        var list = await _fixture.FindAsync<TodoList>(id);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list.CreatedBy.Should().Be(userId);
        list.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(10000));
    }
}
