using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class QuoteDemoController(IQuoteService quoteService) : ControllerBase
{
    [HttpGet("get-quotes")]
    [AllowAnonymous]
    public async Task<IActionResult> GetQuotes([FromQuery]GetQuoteFilter filter)
    {
        var result = await quoteService.GetQuotes(filter);
        return StatusCode(result.StatusCode,result);
    }
}