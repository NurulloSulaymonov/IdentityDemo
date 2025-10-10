using System.ComponentModel.DataAnnotations;

namespace WebApi.Filters;

public class GetPermissionFilter : PaginationFilter
{
    [Required]
    public string RoleId { get; set; }

    public GetPermissionFilter():base()
    {
        
    }
    public string? Search { get; set; }
    public GetPermissionFilter(int pageNumber, int pageSize) : base(pageNumber, pageSize)
    {
        
    }
}