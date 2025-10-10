using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.Account;
using WebApi.Filters;
using WebApi.Services;
using WebApi.Response;
namespace WebApi.Controllers;

[Route("/api/[controller]")]
[Authorize]
public class AccountController(IAccountService accountService,IQuoteService quoteService) : ControllerBase
{
    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
    {
            var response  = await accountService.Register(registerDto);
            return StatusCode(response.StatusCode, response);
    }
    
    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody]LoginDto registerDto)
    {
         var response  = await accountService.Login(registerDto);
         return StatusCode(response.StatusCode, response);
        
    }
    
    [HttpGet("get-quotes")]
    [AllowAnonymous]
    public async Task<IActionResult> GetQuotes([FromQuery]GetQuoteFilter filter)
    {
        var result = await quoteService.GetQuotes(filter);
        return StatusCode(result.StatusCode,result);
    }

    [HttpPost("add-user-to-role")]
    [AllowAnonymous]
    
    public async Task<Response<string>> AddUserToRole([FromBody]UserRoleDto userRoleDto)
    {
        return await accountService.AddRoleToUser(userRoleDto);
    }
    
    
    [HttpDelete("delete-user-from-role")]
    [AllowAnonymous]
    
    public async Task<Response<string>> DeleteRoleFromUser(UserRoleDto userRoleDto)
    {
        return await accountService.RemoveRoleFromUser(userRoleDto);
    }

  
    [HttpDelete("ForgotPassword")]
    [AllowAnonymous]
    public async Task<Response<string>> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        return await accountService.ForgotPasswordTokenGenerator(forgotPasswordDto);
    }
    
      
    [HttpDelete("ResetPassword")]
    [AllowAnonymous]
    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        return await accountService.ResetPassword(resetPasswordDto);
    }
    
    



}
