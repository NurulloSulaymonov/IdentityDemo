using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.Permissions;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("[controller]")]
public class QuoteDemoController(IQuoteService quoteService) : ControllerBase
{
    [HttpGet("get-quotes")]
    [PermissionAuthorize(PermissionConstants.Quotes.View)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetQuotes([FromQuery]GetQuoteFilter filter)
    {
        var result = await quoteService.GetQuotes(filter);
        return StatusCode(result.StatusCode,result);
    }
}