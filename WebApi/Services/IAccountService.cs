using WebApi.Dtos.Account;
using WebApi.Response;

namespace WebApi.Services;

public interface IAccountService
{
    Task<Response<RegisterDto>> Register(RegisterDto model);
    Task<Response<string>> Login(LoginDto login);
    Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto);
    Task<Response<string>> ForgotPasswordTokenGenerator(ForgotPasswordDto forgotPasswordDto);
    Task<Response<string>> ChangePassword(ChangePasswordDto passwordDto, string userId);
    Task<Response<string>> AddRoleToUser(UserRoleDto userRole);
    Task<Response<string>> RemoveRoleFromUser(UserRoleDto userRole);
}