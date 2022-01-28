using System.Security.Claims;
using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string> GetUserNameAsync(Guid? userId);
    Task<bool> IsExistsUser(string userName);
    Task<bool> CheckPassword(string userName, string password);
    Task<IEnumerable<string>> GetRoles(string userName);
    Task<Guid> GetUserId(string userName);
    Task<bool> IsInRoleAsync(Guid? userId, string role);
    Task<bool> AuthorizeAsync(Guid? userId, string policyName);
    Task<(Result Result, Guid UserId)> CreateUserAsync(string userName, string password);
    Task<Result> DeleteUserAsync(Guid userId);
    Task<string> GetJwtToken(IEnumerable<Claim>claims);
}