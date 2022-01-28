using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitecture.Application.Auth.Commands.Login;

public record LoginCommand : IRequest<TokenVm>
{
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenVm>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<TokenVm> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var isExistsUser = await _identityService.IsExistsUser(request.Username);
        if (isExistsUser && await _identityService.CheckPassword(request.Username, request.Password))
        {
            var roles = await _identityService.GetRoles(request.Username);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier,
                    (await _identityService.GetUserId(request.Username)).ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return new TokenVm {Token = await _identityService.GetJwtToken(claims)};
        }

        throw new UnauthorizedAccessException();
    }
}