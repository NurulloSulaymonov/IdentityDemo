using WebApi.Dtos.RoleClaims;
using WebApi.Filters;
using WebApi.Response;

namespace WebApi.Services.Permission;

public interface IPermissionService
{
    Task<PagedResponse<List<RoleClaimDto>>> GetPermissionsByRoleId(GetRolePermissionFilter filter);
    Task<Response<RoleClaimDto>> UpdatePermission(RoleClaimDto permission);
    Task<Response<List<RoleDto>>> GetRoles();
}