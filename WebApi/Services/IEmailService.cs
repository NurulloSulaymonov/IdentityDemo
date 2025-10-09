using MimeKit.Text;
using WebApi.Dtos;

namespace WebApi.Services;

public interface IEmailService
{
    void SendEmail(MessageDto model,TextFormat format);
}