﻿using System.Reflection;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using MediatR;

namespace CleanArchitecture.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated user
            if (_currentUserService.UserId == null)
            {
                throw new UnauthorizedAccessException();
            }

            // Role-based authorization
            await RoleBasedAuthorization(authorizeAttributes);

            // Policy-based authorization
            await PolicyBasedAuthorization(authorizeAttributes);
        }

        // User is authorized / authorization not required
        return await next();
    }

    private async Task RoleBasedAuthorization(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .ToList();

        if (authorizeAttributesWithRoles.Any())
        {
            var authorized = false;

            foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
            {
                foreach (var role in roles)
                {
                    var isInRole = await _identityService.IsInRoleAsync(_currentUserService.UserId, role.Trim());
                    if (!isInRole)
                    {
                        continue;
                    }
                    authorized = true;
                    break;
                }
            }

            // Must be a member of at least one role in roles
            if (!authorized)
            {
                throw new ForbiddenAccessException();
            }
        }
    }

    private async Task PolicyBasedAuthorization(IEnumerable<AuthorizeAttribute> authorizeAttributes)
    {
        var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy))
            .ToList();
        if (authorizeAttributesWithPolicies.Any())
        {
            foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
            {
                var authorized = await _identityService.AuthorizeAsync(_currentUserService.UserId, policy);

                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }
        }
    }
}
