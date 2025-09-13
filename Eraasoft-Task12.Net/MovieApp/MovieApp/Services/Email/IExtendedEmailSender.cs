using Microsoft.AspNetCore.Identity.UI.Services;

namespace MovieApp.Services.Email
{
    public interface IExtendedEmailSender : IEmailSender
    {
        Task SendEmailConfirmationAsync(string email, string name, string confirmationLink);
        Task SendPasswordResetOtpAsync(string email, string name, string otpCode);
    }
}