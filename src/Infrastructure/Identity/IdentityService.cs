using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IConfiguration _configuration;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService, IConfiguration configuration)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _configuration = configuration;
    }

    public async Task<string> GetUserNameAsync(Guid? userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<bool> IsExistsUser(string userName)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == userName);
    }

    public async Task<bool> CheckPassword(string userName, string password)
    {
        var isExistsUser = await IsExistsUser(userName);
        if (!isExistsUser)
        {
            throw new NotFoundException();
        }

        var user = await _userManager.FindByNameAsync(userName);
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IEnumerable<string>> GetRoles(string userName)
    {
        var isExistsUser = await IsExistsUser(userName);
        if (!isExistsUser)
        {
            throw new NotFoundException();
        }

        var user = await _userManager.FindByNameAsync(userName);
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<Guid> GetUserId(string userName)
    {
        var isExistsUser = await IsExistsUser(userName);
        if (!isExistsUser)
        {
            throw new NotFoundException();
        }

        var user = await _userManager.FindByNameAsync(userName);
        return user.Id;
    }

    public async Task<(Result Result, Guid UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser {UserName = userName, Email = userName,};

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(Guid? userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(Guid? userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException();
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(Guid userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public Task<string> GetJwtToken(IEnumerable<Claim> claims)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var token = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            claims,
            DateTime.Now,
            DateTime.Now.AddHours(2),
            new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );
        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
}