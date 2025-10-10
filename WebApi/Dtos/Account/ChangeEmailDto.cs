using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos.Account;

public class ChangeEmailDto
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; }

    [Required]
    public string CurrentPassword { get; set; }
}