namespace WebApi.Dtos.RoleClaims;

public class RoleClaimDto
{
    public string RoleId { get; set; }
    public string Role { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public bool Selected { get; set; }
    
    public RoleClaimDto()
    {
        
    }
    public RoleClaimDto(string type, string value, bool selected)
    {
        Type = type;
        Value = value;
        Selected = selected;
    }
    
    public RoleClaimDto(string type, string value)
    {
        Type = type;
        Value = value;
    }

    public RoleClaimDto(string type, string value, string roleId, string role)
    {
        Type = type;
        Value = value;
        RoleId = roleId;
        Role = role;
    }
}