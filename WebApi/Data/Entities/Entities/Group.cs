namespace WebApi.Data.Entities.Entities;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<StudentGroup> StudentGroups { get; set; }

}