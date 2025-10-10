using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Dtos.Account;
using WebApi.Response;

namespace WebApi.Services;

public class AccountService(
    UserManager<IdentityUser> userManager,
    IConfiguration configuration,
    DataContext context,
    RoleManager<IdentityRole> roleManager,
    IEmailService emailService)
    : IAccountService
{
    private readonly DataContext _context = context;

    public async Task<Response<RegisterDto>> Register(RegisterDto model)
    {
        var mapped = new IdentityUser()
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };
        
        var response = await userManager.CreateAsync(mapped, model.Password);
        if (response.Succeeded == true)
            return new Response<RegisterDto>(model);
        else return new Response<RegisterDto>(HttpStatusCode.BadRequest, response.Errors.Select(e=>e.Description).ToList());

    }
    
    
    public async Task<Response<string>> AddRoleToUser(UserRoleDto userRole)
    {
        var role = await roleManager.FindByIdAsync(userRole.RoleId);
        var user = await userManager.FindByIdAsync(userRole.UserId);
       
        var userInRole = await userManager.IsInRoleAsync(user, role.Name);
        if (userInRole == true) return new Response<string>(HttpStatusCode.BadRequest, "Role exists");
        
        await userManager.AddToRoleAsync(user, role.Name);
        return new Response<string>(HttpStatusCode.OK, "done");
    }
    
    public async Task<Response<string>> RemoveRoleFromUser(UserRoleDto userRole)
    {
        var role = await roleManager.FindByIdAsync(userRole.RoleId);
        var user = await userManager.FindByIdAsync(userRole.UserId);
       
        var userInRole = await userManager.IsInRoleAsync(user, role.Name);
        if (userInRole == false) return new Response<string>(HttpStatusCode.BadRequest, "This user does not have this role");
        
        await userManager.RemoveFromRoleAsync(user, role.Name);
        return new Response<string>(HttpStatusCode.OK, "done");
    }
    

    public async Task<Response<string>> Login(LoginDto login)
    {
        var user = await userManager.FindByNameAsync(login.Username);
        if (user != null)
        {
            var checkPassword = await userManager.CheckPasswordAsync(user, login.Password);
            if (checkPassword)
            {
                var token = await GenerateJwtToken(user);
                return new Response<string>(token);
            }
            else
            {
                return new Response<string>(HttpStatusCode.BadRequest, "login or password is incorrect");
            }

        }

        return new Response<string>(HttpStatusCode.BadRequest, "login or password is incorrect");
    }

    //Method to generate The Token
    private async Task<string> GenerateJwtToken(IdentityUser user)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        //add roles
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );


        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }

    public async Task<Response<string>> ChangePassword(ChangePasswordDto passwordDto, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        var checkPassword = await userManager.CheckPasswordAsync(user!, passwordDto.OldPassword);
        if (checkPassword == false)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "password is incorrect");
        }
        var token = await userManager.GeneratePasswordResetTokenAsync(user!);
        var result = await userManager.ResetPasswordAsync(user!, token, passwordDto.Password);
        if (result.Succeeded == true)
            return new Response<string>(HttpStatusCode.OK, "success");
        else return new Response<string>(HttpStatusCode.BadRequest, "could not reset your password");
    }

    public async Task<Response<string>> ForgotPasswordTokenGenerator(ForgotPasswordDto forgotPasswordDto)
    {
        var existing = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (existing == null) return new Response<string>(HttpStatusCode.BadRequest, "not found");
        
        var token = await userManager.GeneratePasswordResetTokenAsync(existing);
        var url =$"http://localhost:5282/account/resetpassword?token={token}&email={forgotPasswordDto.Email}";
       
        var message = new MessageDto(new[] { forgotPasswordDto.Email }, "reset password",
            $"<h1><a href=\"{url}\">reset password</a></h1>");
        emailService.SendEmail(message,TextFormat.Html);

        return new Response<string>(HttpStatusCode.OK, "reset password has been sent");
    }

    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
            return new Response<string>(HttpStatusCode.BadRequest, "user not found");

        var resetPassResult = await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
        if(resetPassResult.Succeeded)  
            return new Response<string>(HttpStatusCode.OK, "success");
        
        return new Response<string>(HttpStatusCode.BadRequest, "please try again");

    }
    
    


}