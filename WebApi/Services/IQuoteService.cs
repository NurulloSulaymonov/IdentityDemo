using WebApi.Dtos;
using WebApi.Filters;
using WebApi.Response;

namespace WebApi.Services;

public interface IQuoteService
{
    Task<PagedResponse<List<GetQuoteDto>>> GetQuotes(GetQuoteFilter filter);
    Task<Response<GetQuoteDto>> GetQuoteById(int id);
    Task<Response<GetQuoteDto>> AddQuote(AddQuoteDto quoteDto);
    Task<Response<GetQuoteDto>> UpdateQuote(AddQuoteDto quoteDto);
    Task<Response<bool>> DeleteQuote(int id);
    
}