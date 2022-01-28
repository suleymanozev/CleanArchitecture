using CleanArchitecture.Application.IntegrationTests.Common.Fixtures;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application.IntegrationTests.Common.Extensions;

public static class TestServerFixtureExtensions
{
    public static async Task<TResponse> SendAsync<TResponse>(this TestServerFixture fixture,
        IRequest<TResponse> request)
    {
        using var scope = fixture.Services.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task<TEntity?> FindAsync<TEntity>(this TestServerFixture fixture, params object[] keyValues)
        where TEntity : class, IEntity
    {
        using var scope = fixture.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(this TestServerFixture fixture, TEntity entity)
        where TEntity : class, IEntity
    {
        using var scope = fixture.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>(this TestServerFixture fixture) where TEntity : class, IEntity
    {
        using var scope = fixture.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }
    
    public static async Task<Guid> RunAsDefaultUserAsync(this TestServerFixture fixture)
    {
        return await RunAsUserAsync(fixture, "test@local", "Testing1234!", Array.Empty<string>());
    }

    public static async Task<Guid> RunAsAdministratorAsync(this TestServerFixture fixture)
    {
        return await RunAsUserAsync(fixture, "administrator@local", "Administrator1234!", new[] { "Administrator" });
    }

    public static async Task<Guid> RunAsUserAsync(this TestServerFixture fixture, string userName, string password, string[] roles)
    {
        using var scope = fixture.Services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = userName, Email = userName };

        var result = await userManager.CreateAsync(user, password);

        if (roles.Any())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new ApplicationRole(role));
            }

            await userManager.AddToRolesAsync(user, roles);
        }

        if (result.Succeeded)
        {
            fixture.MockCurrentUserService.SetupGet(s => s.UserId).Returns(user.Id);
            return user.Id;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }
}