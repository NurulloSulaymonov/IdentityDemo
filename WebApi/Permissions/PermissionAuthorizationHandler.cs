using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Data;

namespace WebApi.Permissions;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<PermissionAuthorizationHandler> _logger;
    private readonly DataContext _context;

    public PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    // Check whether a given PermissionRequirement is satisfied or not for a particular context
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        _logger.LogWarning("Evaluating authorization requirement for permission {permission}", requirement.Permission);
        var user = context.User;   
        // var userId = user.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.NameIdentifier)?.Value;
        // if(userId == null)
        //     return Task.CompletedTask;

        foreach (Claim claim in context.User.Claims)
        {
           // if (claim.Type != "Permissions" || claim.Value != requirement.Permission)
             //   continue;
            
            if (claim.Type == "Permissions" && claim.Value == requirement.Permission)
            {
                _logger.LogInformation("Permission {permission} is satisfied", requirement.Permission);
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }
}