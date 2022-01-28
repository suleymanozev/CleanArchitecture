using CleanArchitecture.Application.Auth.Commands.Login;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.IntegrationTests.Common.Extensions;
using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CleanArchitecture.Application.IntegrationTests.Auth;

[Collection(nameof(ApiTestCollection))]
public class LoginTests : BaseScenario
{
    private readonly TestServerFixture _fixture;

    public LoginTests(TestServerFixture fixture) : base(fixture)
    {
        _fixture = fixture;
    }


    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new LoginCommand();

        await FluentActions.Invoking(() =>
            _fixture.SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ShouldLogin()
    {
        await _fixture.RunAsDefaultUserAsync();
        var command = new LoginCommand {Username = "test@local", Password = "Testing1234!"};
        await FluentActions.Invoking(async () =>
        {
            var tokenVm = await _fixture.SendAsync(command);
            tokenVm.Should().NotBeNull();
            tokenVm.Token.Should().NotBeNullOrEmpty();
        }).Should().NotThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task ShouldNotLogin()
    {
        var command = new LoginCommand {Username = "test@local", Password = "Testing1234!"};
        await FluentActions.Invoking(() => _fixture.SendAsync(command)).Should()
            .ThrowAsync<UnauthorizedAccessException>();
    }
}