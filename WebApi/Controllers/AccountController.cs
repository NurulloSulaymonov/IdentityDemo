using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.Account;
using WebApi.Filters;
using WebApi.Services;
using WebApi.Response;
namespace WebApi.Controllers;

[Route("[controller]")]
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
    [Produces("application/json")]
    public async Task<IActionResult> GetQuotes([FromQuery]GetQuoteFilter filter)
    {
        var result = await quoteService.GetQuotes(filter);
        return StatusCode(result.StatusCode,result);
    }

    [HttpPost("AddUserToRole")]
    public async Task<Response<string>> AddUserToRole(UserRoleDto userRoleDto)
    {
        return await accountService.AddOrRemoveUserFromRole(userRoleDto,false);
    }
    
    
    [HttpDelete("DeleteRoleFromUser")]
    public async Task<Response<string>> DeleteRoleFromUser(UserRoleDto userRoleDto)
    {
        return await accountService.AddOrRemoveUserFromRole(userRoleDto,true);
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
