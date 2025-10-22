using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.Permissions;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
// [Authorize(Roles = "Admin")]
[Route("[controller]")]
public class QuoteDemoController(IQuoteService quoteService, ILogger<QuoteDemoController> logger) : ControllerBase
{
    [HttpGet("get-quotes")]
    // [PermissionAuthorize(PermissionConstants.Quotes.View)]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetQuotes([FromQuery]GetQuoteFilter filter)
    {
        
        logger.LogTrace("This is from trace");
        logger.LogDebug("This is from debug level");
        logger.LogInformation("This is from information level");
        logger.LogWarning("This is from warning level");
        
        var result = await quoteService.GetQuotes(filter);
        return StatusCode(result.StatusCode,result);
        
        
    }
}