namespace WebApi.Dtos.Account;

public class EmailConfiguration
{
    public const string SectionName = "JWT";
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}