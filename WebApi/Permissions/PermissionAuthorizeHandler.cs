using Microsoft.AspNetCore.Authorization;

namespace WebApi.Permissions;

public class PermissionAuthorizeHandler:AuthorizationHandler<PermissionRequirement>
{

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        foreach(var claim in context.User.Claims)
        {
            if (claim.Type == "Permissions" && claim.Value.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}