namespace WebApi.Data.Entities.Entities;

public class Student
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public List<StudentGroup> StudentGroups { get; set; }
}