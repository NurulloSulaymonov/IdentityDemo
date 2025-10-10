using Microsoft.AspNetCore.Authorization;

namespace WebApi.Permissions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    public PermissionAuthorizeAttribute(string requiredPermission)
    {
        AuthenticationSchemes = "Bearer";
        Policy = requiredPermission;
    }
        
}