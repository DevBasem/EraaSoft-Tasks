using Microsoft.AspNetCore.Identity.UI.Services;
using MovieApp.Models;
using System.Net;
using System.Net.Mail;

namespace MovieApp.Services.Email
{
    public class EmailSender : IEmailSender, IExtendedEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ILogger<EmailSender> _logger;
        private readonly IEmailTemplateRenderer _templateRenderer;

        public EmailSender(
            EmailConfiguration emailConfig, 
            ILogger<EmailSender> logger,
            IEmailTemplateRenderer templateRenderer)
        {
            _emailConfig = emailConfig;
            _logger = logger;
            _templateRenderer = templateRenderer;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new EmailMessage
                {
                    ToAddresses = new List<string> { email },
                    Subject = subject,
                    Content = htmlMessage
                };

                await SendAsync(message);
                _logger.LogInformation($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {email}: {ex.Message}");
                throw;
            }
        }
        
        public async Task SendEmailConfirmationAsync(string email, string name, string confirmationLink)
        {
            try
            {
                var model = new 
                {
                    Name = name,
                    ConfirmationLink = confirmationLink
                };
                
                var htmlContent = await _templateRenderer.RenderEmailTemplateAsync("EmailConfirmation", model);
                
                var message = new EmailMessage
                {
                    ToAddresses = new List<string> { email },
                    Subject = "Confirm Your MovieApp Account",
                    Content = htmlContent
                };

                await SendAsync(message);
                _logger.LogInformation($"Confirmation email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending confirmation email to {email}: {ex.Message}");
                throw;
            }
        }
        
        public async Task SendPasswordResetOtpAsync(string email, string name, string otpCode)
        {
            try
            {
                var model = new 
                {
                    Name = name,
                    OtpCode = otpCode
                };
                
                var htmlContent = await _templateRenderer.RenderEmailTemplateAsync("PasswordResetOtp", model);
                
                var message = new EmailMessage
                {
                    ToAddresses = new List<string> { email },
                    Subject = "Your MovieApp Password Reset Code",
                    Content = htmlContent
                };

                await SendAsync(message);
                _logger.LogInformation($"Password reset OTP email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending password reset OTP email to {email}: {ex.Message}");
                throw;
            }
        }

        private async Task SendAsync(EmailMessage message)
        {
            using var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        private MailMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MailMessage
            {
                Subject = message.Subject,
                Body = message.Content,
                IsBodyHtml = true,
                From = new MailAddress(_emailConfig.From)
            };

            foreach (var to in message.ToAddresses)
            {
                emailMessage.To.Add(new MailAddress(to));
            }

            return emailMessage;
        }

        private async Task SendAsync(MailMessage mailMessage)
        {
            using var client = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.Port)
            {
                EnableSsl = _emailConfig.EnableSsl,
                Credentials = new NetworkCredential(_emailConfig.UserName, _emailConfig.Password)
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}