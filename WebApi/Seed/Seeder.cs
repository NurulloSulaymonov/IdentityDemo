using Microsoft.AspNetCore.Identity;
using WebApi.Data;

namespace WebApi.Seed;

public class Seeder
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public Seeder(DataContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
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

    public async Task SeedUser()
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
    
   

}

public class Roles
{
    public const string Admin = "Admin";
    public const string Marketing = "Marketing";
    public const string Finance = "Finance";
    public const string Mentor = "Mentor";
    public const string Student = "Student";
}