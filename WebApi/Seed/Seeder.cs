using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Data;
using WebApi.Dtos.RoleClaims;

namespace WebApi.Seed;

public class Seeder
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<Seeder> _logger;
    private readonly IMemoryCache _memoryCache;

    public Seeder(DataContext context, UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager,
        ILogger<Seeder>logger, IMemoryCache memoryCache)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task SeedRole()
    {
        var newroles = new List<IdentityRole>()
        {
            new (Roles.Admin),
            new (Roles.Marketing),
            new (Roles.Finance),
            new (Roles.Mentor),
            new (Roles.Student),

        };

        var existing = _roleManager.Roles.ToList();
        foreach (var role in newroles)
        {
            if (existing.Exists(e => e.Name == role.Name) == false)
            {
                await _roleManager.CreateAsync(role);
            }
        }
    }


    public async Task RestorePermissions()
    {
        var roles = _roleManager.Roles.ToList();
        var permissionsInCache = new List<RoleClaimDto>();
        foreach (var role in roles)
        {
            var permissions = await _roleManager.GetClaimsAsync(role);
            permissionsInCache.AddRange(permissions.Select(e=>new RoleClaimDto(e.Type,e.Value,role.Id,role.Name!)));
        }

        _memoryCache.Set("permissions", permissionsInCache);
    }

    public async Task SeedUser()
    {
        try
        {
            var existing = await _userManager.FindByNameAsync("admin");
            if (existing is not null) return;

            var identity = new IdentityUser()
            {
                UserName = "admin",
                PhoneNumber = "13456777",
                Email = "admin@gmail.com"
            };

            var result = await _userManager.CreateAsync(identity, "hello123");
            await _userManager.AddToRoleAsync(identity, Roles.Admin);
            return;
        }
        catch (Exception ex)
        {
           _logger.LogError(ex.StackTrace);
        }
    }
    
   

}

public class Roles
{
    public const string Admin = "Admin";
    public const string Marketing = "Marketing";
    public const string Finance = "Finance";
    public const string Mentor = "Mentor";
    public const string Student = "Student";
}