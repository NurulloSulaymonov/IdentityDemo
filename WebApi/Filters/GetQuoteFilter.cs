namespace WebApi.Filters;

public class GetQuoteFilter : PaginationFilter
{
    public string? Author { get; set; } 
    public string? QuoteText { get; set; } 
}