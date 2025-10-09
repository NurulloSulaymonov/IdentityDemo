using WebApi.Data.Entities.Entities;

namespace WebApi.Dtos;

public class GetQuoteDto : QuoteDto
{

    public string CategoryName { get; set; }
    
    public GetQuoteDto(int id, string? author, string text, string? imageName)
    {
        Id = id;
        AuthorName = author;
        Title = text;
        ImageName = imageName;
    }

    public GetQuoteDto(Quote quote)
    {
        Id = quote.Id;
        Title = quote.QuoteText;
        AuthorName = quote.Author;
        ImageName = quote.ImageName;
    }

    public DateTime CreatedAt { get; set; }
}