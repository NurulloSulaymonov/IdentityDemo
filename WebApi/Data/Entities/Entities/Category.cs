namespace WebApi.Data.Entities.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Quote> Quotes { get; set; }
}