using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.Entities.Entities;

public class Quote
{
    [Key]
    public int Id { get; set; }
    [MaxLength(200)]
    public string QuoteText { get; set; }
    [MaxLength(50)] 
    public string? Author { get; set; }
    [MaxLength(100)]
    public string? ImageName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CategoryId { get; set; }
    public Category Category { get; set; }
}