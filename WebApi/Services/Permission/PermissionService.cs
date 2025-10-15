using System.Net;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Data;
using WebApi.Dtos.RoleClaims;
using WebApi.Filters;
using WebApi.Permissions;
using WebApi.Response;

namespace WebApi.Services.Permission;

public class PermissionService : IPermissionService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly DataContext _context;
    private readonly IMemoryCache _cache;

    public PermissionService(RoleManager<IdentityRole> roleManager, DataContext context, IMemoryCache cache)
    {
        _roleManager = roleManager;
        _context = context;
        _cache = cache;
    } 
    
    //get all permissions by roleId
    public async Task<PagedResponse<List<RoleClaimDto>>> GetPermissionsByRoleId(GetRolePermissionFilter filter)
    {
        var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
        var role = await _roleManager.FindByIdAsync(filter.RoleId);
        
        var allPermissions = GetPermissions(typeof(PermissionConstants));
        
        var roleClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in allPermissions)
        {
            permission.RoleId = role.Id;
            permission.Selected = roleClaims.Any(c => c.Value == permission.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            string searchTerm = filter.Search.ToLower();
            allPermissions = allPermissions
                .Where(p => p.Value.ToLower().Contains(searchTerm))
                .ToList();
        }
        
        //pagination
        var totalRecords = allPermissions.Count;
        var pagedPermissions = allPermissions
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToList();
        
        return new PagedResponse<List<RoleClaimDto>>(
            filter.PageNumber, 
            filter.PageSize,totalRecords,
            pagedPermissions);

    }
    
    //update permissions by roleId
    public async Task<Response<RoleClaimDto>> UpdatePermission( RoleClaimDto permission)
    {
        if (string.IsNullOrWhiteSpace(permission.RoleId))
            return new Response<RoleClaimDto>(HttpStatusCode.BadRequest,"Invalid permission data.");

        var role = await _roleManager.FindByIdAsync(permission.RoleId);
        if (role == null)
            return new Response<RoleClaimDto>(HttpStatusCode.BadRequest,$"Role with ID '{permission.RoleId}' not found.");

        var claims = await _roleManager.GetClaimsAsync(role);
        var existing = claims.FirstOrDefault(c => c.Type == permission.Type && c.Value == permission.Value);
        

        if (permission.Selected)
        {
            // Add if not exists
            if (existing == null)
            {
                permission.RoleId = role.Id;
               var  result = await _roleManager.AddClaimAsync(role, new Claim(permission.Type, permission.Value));
               if (result.Succeeded)
                   UpdateRoleClaimInCache(permission);
                
               if (!result.Succeeded)
                    return new Response<RoleClaimDto>(HttpStatusCode.InternalServerError, result.Errors.Select(e => e.Description).ToList());
            }
        }
        else
        {
            // Remove if exists
            if (existing != null)
            {
                var result = await _roleManager.RemoveClaimAsync(role, existing); // use existing claim!
                if (result.Succeeded)
                    UpdateRoleClaimInCache(permission, deleted: true);

                if (!result.Succeeded)
                    return new Response<RoleClaimDto>(HttpStatusCode.InternalServerError, result.Errors.Select(e => e.Description).ToList());
            }
        }

        return new Response<RoleClaimDto>(permission);  
    }

    private void UpdateRoleClaimInCache(RoleClaimDto permission, bool deleted=false)
    {
        const string cacheKey = "permissions";

        if (_cache.TryGetValue(cacheKey, out List<RoleClaimDto> permissions))
        {
            if (deleted)
            {
                var existing =
                    permissions.FirstOrDefault(e => e.Type == permission.Type && e.Value == permission.Value);
                if (existing is not null)
                {
                    permissions.Remove(existing);
                }
            }
            if (!permissions.Any(e => e.Value == permission.Value))
            {
                permissions.Add(permission);
            }
            _cache.Set(cacheKey, permissions);
        }
    }

    public async Task<Response<List<RoleDto>>> GetRoles()
    {
        
        var roles = await _roleManager.Roles.Select(x=>new RoleDto(){Id = x.Id, Name = x.Name}).ToListAsync();
        return new Response<List<RoleDto>>(roles);
    }
    
    public List<RoleClaimDto> GetPermissions(Type policy)
    {
        var nestedTypes = policy.GetNestedTypes(BindingFlags.Public);
        var  allPermissions = new List<RoleClaimDto>();
        if (nestedTypes.Length > 0)
        {
            foreach (var nested in nestedTypes)
            {
                FieldInfo[] fields = nested.GetFields(BindingFlags.Static | BindingFlags.Public);

                foreach (FieldInfo fi in fields)
                {
                    allPermissions.Add(new RoleClaimDto("Permissions", fi.GetValue(null).ToString()));
                }
            }
        }
        else
        { 
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo fi in fields)
            {
                allPermissions.Add(new RoleClaimDto(fi.GetValue(null).ToString(), "Permissions"));
            }
        }

        return allPermissions;

    }
}