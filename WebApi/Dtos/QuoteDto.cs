using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos;

public class QuoteDto
{
    
    public int Id { get; set; }
    [Required, MinLength(5)]
    public string Title { get; set; }
    [Required]
    public string? AuthorName { get; set; }
    public string? ImageName { get; set; }
    public int CategoryId { get; set; }

}