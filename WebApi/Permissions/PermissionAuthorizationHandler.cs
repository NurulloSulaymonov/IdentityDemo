using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Data;
using WebApi.Dtos.RoleClaims;

namespace WebApi.Permissions;

public class PermissionAuthorizationHandler(
    ILogger<PermissionAuthorizationHandler> logger,
    IMemoryCache cache)
    : AuthorizationHandler<PermissionRequirement>
{

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var user = context.User;
        var role = user.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Role);
        var permissionInClaims = user.Claims.Where(e => e.Type == "Permissions");
        if (cache.TryGetValue("permissions", out List<RoleClaimDto> permissions))
        {
            permissions = permissions.Where(e => e.Role == role!.Value).ToList();
            foreach (var e in permissionInClaims)
            {
                if (permissions.Any(e=>e.Value == requirement.Permission))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
        }
        return Task.CompletedTask;
    }
}