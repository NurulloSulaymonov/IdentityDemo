using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Filters;
using WebApi.Permissions;
using WebApi.Response;
using WebApi.Services;

namespace WebApi.Controllers;



[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class QuoteController(IQuoteService quoteService) : ControllerBase
{
    [HttpGet("get-quotes")]
    [AllowAnonymous]
    public async Task<IActionResult> GetQuotes([FromQuery]GetQuoteFilter filter)
    {
        var result = await quoteService.GetQuotes(filter);
        return StatusCode(result.StatusCode,result);
    }
    
    
    [HttpPost("add-quote")]
    public async Task<IActionResult> AddQuote([FromBody]AddQuoteDto quoteDto)
    {
        if (ModelState.IsValid)
        {
            var response = await quoteService.AddQuote(quoteDto);
            return StatusCode(response.StatusCode, response);
        }
        else
        {
            var errors = ModelState.SelectMany(e => e.Value.Errors.Select(er=>er.ErrorMessage)).ToList();
            var response  =  new Response<GetQuoteDto>(HttpStatusCode.BadRequest, errors);
            return StatusCode(response.StatusCode, response);
        }
    }
    
    
    [HttpPut("update-quote")]
    public async Task<Response<GetQuoteDto>> UpdateQuote(AddQuoteDto quoteDto)
    {
        return await quoteService.UpdateQuote(quoteDto);
    }
    
    [HttpDelete("delete-quote")]
    [PermissionAuthorize("Quote.Delete")]
    public async Task<IActionResult> DeleteQuote(int quoteId)
    {
      var response = await quoteService.DeleteQuote(quoteId);
      return StatusCode(response.StatusCode, response);
    }
}