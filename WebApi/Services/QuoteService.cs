using System.Net;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Entities.Entities;
using WebApi.Dtos;
using WebApi.Filters;
using WebApi.Response;

namespace WebApi.Services;

public class QuoteService : IQuoteService
{
    private readonly DataContext _context;
    

    public QuoteService(DataContext context)
    {
        _context = context;
    }
    public async Task<PagedResponse<List<GetQuoteDto>>> GetQuotes(GetQuoteFilter filter)
    {
        
        var quotes =  _context.Quotes.AsQueryable();

        if (string.IsNullOrEmpty(filter.Author) == false)
            quotes = quotes.Where(q => q.Author.ToLower().Contains(filter.Author.ToLower()));
        
        if (string.IsNullOrEmpty(filter.QuoteText) == false)
            quotes = quotes.Where(q => q.Category.Name.ToLower().Contains(filter.QuoteText.ToLower()));

        var totalRecords = quotes.Count();
        var mapped = quotes.Select(e => new GetQuoteDto(e)).ToList();
        return new PagedResponse<List<GetQuoteDto>>(filter.PageNumber,filter.PageSize,totalRecords,mapped);
        
    }

    public async Task<Response<GetQuoteDto>> GetQuoteById(int id)
    {
    
        var quote = await _context.Quotes.FirstOrDefaultAsync(q => q.Id == id);
        var mapped = new GetQuoteDto(quote);
        
        return new Response<GetQuoteDto>(mapped) ;
    }

    public async Task<Response<GetQuoteDto>> AddQuote(AddQuoteDto quoteDto)
    {
        var quote = new Quote()
        {
            QuoteText = quoteDto.Title,
            Author = quoteDto.AuthorName,
            CategoryId = quoteDto.CategoryId
        };

        await _context.Quotes.AddAsync(quote);
       await _context.SaveChangesAsync();

       var mapped = new GetQuoteDto(quote);
       return new Response<GetQuoteDto>(mapped);
    }

    public async Task<Response<GetQuoteDto>> UpdateQuote(AddQuoteDto quoteDto)
    {
        var quote = new Quote()
        {
            QuoteText = quoteDto.Title,
            Author = quoteDto.AuthorName,
            CategoryId = quoteDto.CategoryId
        };
        _context.Quotes.Update(quote);
        await _context.SaveChangesAsync();

        var mapped = new GetQuoteDto(quote);
         return new Response<GetQuoteDto>(mapped);

    }

    public async Task<Response<bool>> DeleteQuote(int id)
    {
        var existing = await _context.Quotes.FindAsync(id);
        if (existing == null) return new Response<bool>(HttpStatusCode.BadRequest,"Quote not found");
        
        _context.Quotes.Remove(existing);
        await _context.SaveChangesAsync();
        return new Response<bool>(true);

    }
}