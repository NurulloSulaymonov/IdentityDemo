using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.RoleClaims;
using WebApi.Permissions;
using WebApi.Response;
using WebApi.Services.Permission;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RoleController(IPermissionService permissionService) :ControllerBase
{
    //get all roles
    [HttpGet("roles")]
    // [PermissionAuthorize(PermissionConstants.Roles.View)]
    public async Task<Response<List<RoleDto>>> GetRoles()
    {
        return await permissionService.GetRoles();
    }

    [HttpGet("permissions")]
    // [PermissionAuthorize(PermissionConstants.Roles.View)]
    public async Task<PagedResponse<List<RoleClaimDto>>> GetPermissions([FromQuery] GetRolePermissionFilter filter)
    {
        return await permissionService.GetPermissionsByRoleId(filter);
    }

    [HttpPut("update-role-permissions")]
    // [PermissionAuthorize(PermissionConstants.Roles.Edit)]
    public async Task<Response<RoleClaimDto>> UpdateRolePermission(RoleClaimDto permission)
    {
            return await permissionService.UpdatePermission(permission);
    }
}