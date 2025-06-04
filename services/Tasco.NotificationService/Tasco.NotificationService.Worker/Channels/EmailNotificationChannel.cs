using Microsoft.Extensions.Logging;
using System.Net.Mail;
using Tasco.NotificationService.Worker.SMTPs.Repositories;
using Tasco.NotificationService.Worker.Channels.Interface;
using Tasco.NotificationService.Worker.Services.Interfaces;
using Tasco.NotificationService.Worker.Models;

namespace Tasco.NotificationService.Worker.Channels
{
    public class EmailNotificationChannel : IEmailNotificationChannel
    {
        private readonly ILogger<EmailNotificationChannel> _logger;
        private readonly INotificationTemplateService _templateService;
        private readonly IEmailRepository _emailRepository;

        public EmailNotificationChannel(
            ILogger<EmailNotificationChannel> logger,
            INotificationTemplateService templateService,
            IEmailRepository emailRepository)
        {
            _logger = logger;
            _templateService = templateService;
            _emailRepository = emailRepository;
        }

        public NotificationChannel ChannelType => NotificationChannel.Email;

        public bool CanHandle(NotificationChannel channel)
        {
            return channel == NotificationChannel.Email;
        }

        public async Task<ChannelResult> SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default)
        {
            var result = new ChannelResult
            {
                IsSuccess = false,
                SentAt = DateTime.UtcNow
            };

            try
            {
                if (notification == null)
                {
                    result.ErrorMessage = "Notification message is null";
                    _logger.LogError("Notification message is null");
                    return result;
                }

                // Get user email from metadata or use UserId as email
                var userEmail = GetUserEmail(notification);
                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    result.ErrorMessage = "User email not found";
                    _logger.LogError("User email not found for notification {NotificationId}", notification.Id);
                    return result;
                }

                // Generate email content using template service
                var emailContent = await GenerateEmailContentAsync(notification);
                
                // Send email
                await SendEmailAsync(userEmail, emailContent.Subject, emailContent.Body, cancellationToken);

                result.IsSuccess = true;
                result.ExternalId = notification.Id; // Use notification ID as external reference
                
                _logger.LogInformation("Email sent successfully for notification {NotificationId} to {Email}", 
                    notification.Id, userEmail);

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Failed to send email for notification {NotificationId}", notification.Id);
                return result;
            }
        }

        public async Task<ChannelResult> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            var result = new ChannelResult
            {
                IsSuccess = false,
                SentAt = DateTime.UtcNow
            };

            try
            {
                if (string.IsNullOrWhiteSpace(to))
                {
                    result.ErrorMessage = "Recipient email is required";
                    _logger.LogError("Recipient email is required");
                    return result;
                }

                if (string.IsNullOrWhiteSpace(subject))
                {
                    result.ErrorMessage = "Email subject is required";
                    _logger.LogError("Email subject is required");
                    return result;
                }

                using var mailMessage = new MailMessage();
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true; // Support HTML content

                await _emailRepository.SendEmailAsync(mailMessage);

                result.IsSuccess = true;
                result.ExternalId = Guid.NewGuid().ToString(); // Generate unique ID for tracking
                
                _logger.LogInformation("Email sent successfully to {Email} with subject: {Subject}", to, subject);

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Failed to send email to {Email}", to);
                return result;
            }
        }

        private string GetUserEmail(NotificationMessage notification)
        {
            try
            {
                // Try to get email from metadata first
                if (notification.Metadata != null && notification.Metadata.ContainsKey("email"))
                {
                    return notification.Metadata["email"]?.ToString() ?? string.Empty;
                }

                // Try to get email from metadata with different keys
                if (notification.Metadata != null && notification.Metadata.ContainsKey("userEmail"))
                {
                    return notification.Metadata["userEmail"]?.ToString() ?? string.Empty;
                }

                // If no email in metadata, assume UserId is an email
                if (IsValidEmail(notification.UserId))
                {
                    return notification.UserId;
                }

                // Last resort: append default domain if UserId looks like username
                if (!string.IsNullOrWhiteSpace(notification.UserId) && !notification.UserId.Contains("@"))
                {
                    _logger.LogWarning("UserId {UserId} is not an email, consider adding email to metadata", notification.UserId);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user email from notification {NotificationId}", notification.Id);
                return string.Empty;
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task<(string Subject, string Body)> GenerateEmailContentAsync(NotificationMessage notification)
        {
            try
            {
                // Use template service if available
                if (_templateService != null)
                {
                    var templateResult = await _templateService.GenerateContentAsync(
                        NotificationChannel.Email, 
                        notification.Type, 
                        notification);

                    if (templateResult.IsSuccess)
                    {
                        return (templateResult.Subject, templateResult.Body);
                    }
                    
                    _logger.LogWarning("Template service failed, using default content for notification {NotificationId}: {Error}", 
                        notification.Id, templateResult.ErrorMessage);
                }

                // Fallback to default content generation
                return GenerateDefaultEmailContent(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating email content for notification {NotificationId}", notification.Id);
                return GenerateDefaultEmailContent(notification);
            }
        }

        private static (string Subject, string Body) GenerateDefaultEmailContent(NotificationMessage notification)
        {
            var subject = $"[{notification.Type}] {notification.Title}";
            
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 10px;'>
                            {notification.Title}
                        </h2>
                        <div style='background: #f8f9fa; padding: 15px; border-left: 4px solid #3498db; margin: 20px 0;'>
                            <p style='margin: 0; font-size: 16px;'>{notification.Message}</p>
                        </div>
                        <div style='margin-top: 20px; padding: 15px; background: #ffffff; border: 1px solid #e9ecef;'>
                            <p style='margin: 0; color: #6c757d; font-size: 14px;'>
                                <strong>Type:</strong> {notification.Type}<br>
                                <strong>Priority:</strong> {notification.Priority}<br>
                                <strong>Created:</strong> {notification.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC
                            </p>
                        </div>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #e9ecef;'>
                        <p style='color: #6c757d; font-size: 12px; text-align: center; margin: 0;'>
                            This is an automated notification from Tasco System.
                        </p>
                    </div>
                </body>
                </html>";

            return (subject, body);
        }
    }
} 