using Microsoft.Extensions.Logging;
using Tasco.NotificationService.Worker.Services.Interfaces;
using Tasco.NotificationService.Worker.Models;

namespace Tasco.NotificationService.Service.Services
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly ILogger<NotificationTemplateService> _logger;

        public NotificationTemplateService(ILogger<NotificationTemplateService> logger)
        {
            _logger = logger;
        }

        public async Task<TemplateResult> GenerateContentAsync(
            NotificationChannel channel, 
            NotificationType type, 
            NotificationMessage notification, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.CompletedTask; // Simulate async operation

                if (channel != NotificationChannel.Email)
                {
                    return new TemplateResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Template service doesn't support channel: {channel}"
                    };
                }

                var (subject, body) = GenerateEmailTemplate(type, notification);

                return new TemplateResult
                {
                    IsSuccess = true,
                    Subject = subject,
                    Body = body
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating template for notification {NotificationId}", notification.Id);
                return new TemplateResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private static (string Subject, string Body) GenerateEmailTemplate(NotificationType type, NotificationMessage notification)
        {
            var subject = type switch
            {
                NotificationType.TaskStatusChanged => $"Task Status Updated: {notification.Title}",
                NotificationType.TaskAssigned => $"New Task Assigned: {notification.Title}",
                NotificationType.TaskCommentAdded => $"New Comment on Task: {notification.Title}",
                NotificationType.ProjectCreated => $"New Project Created: {notification.Title}",
                NotificationType.ProjectUpdated => $"Project Updated: {notification.Title}",
                NotificationType.DeadlineReminder => $"⏰ Deadline Reminder: {notification.Title}",
                NotificationType.MentionInComment => $"You were mentioned in: {notification.Title}",
                _ => $"[{type}] {notification.Title}"
            };

            var body = GenerateEmailBody(type, notification);

            return (subject, body);
        }

        private static string GenerateEmailBody(NotificationType type, NotificationMessage notification)
        {
            var priorityColor = notification.Priority switch
            {
                NotificationPriority.Critical => "#dc3545",
                NotificationPriority.High => "#fd7e14",
                NotificationPriority.Normal => "#0d6efd",
                NotificationPriority.Low => "#6c757d",
                _ => "#0d6efd"
            };

            var priorityIcon = notification.Priority switch
            {
                NotificationPriority.Critical => "🚨",
                NotificationPriority.High => "⚠️",
                NotificationPriority.Normal => "ℹ️",
                NotificationPriority.Low => "📝",
                _ => "ℹ️"
            };

            var typeIcon = type switch
            {
                NotificationType.TaskStatusChanged => "🔄",
                NotificationType.TaskAssigned => "📋",
                NotificationType.TaskCommentAdded => "💬",
                NotificationType.ProjectCreated => "🆕",
                NotificationType.ProjectUpdated => "📝",
                NotificationType.DeadlineReminder => "⏰",
                NotificationType.MentionInComment => "👥",
                _ => "📢"
            };

            var actionUrl = GenerateActionUrl(notification);
            var actionButton = !string.IsNullOrWhiteSpace(actionUrl) 
                ? $@"<div style='text-align: center; margin: 30px 0;'>
                        <a href='{actionUrl}' style='background-color: #0d6efd; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; font-weight: bold;'>
                            View Details
                        </a>
                    </div>" 
                : "";

            var additionalInfo = GenerateAdditionalInfo(notification);

            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f8f9fa;'>
                    <div style='max-width: 600px; margin: 20px auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                        <!-- Header -->
                        <div style='background: linear-gradient(135deg, #0d6efd 0%, #0056b3 100%); color: white; padding: 30px 20px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px; font-weight: 300;'>
                                {typeIcon} Tasco Notification
                            </h1>
                        </div>
                        
                        <!-- Content -->
                        <div style='padding: 30px 20px;'>
                            <!-- Priority Badge -->
                            <div style='margin-bottom: 20px;'>
                                <span style='background-color: {priorityColor}; color: white; padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: bold; text-transform: uppercase;'>
                                    {priorityIcon} {notification.Priority} Priority
                                </span>
                            </div>
                            
                            <!-- Title -->
                            <h2 style='color: #2c3e50; margin: 0 0 20px 0; font-size: 20px; border-bottom: 2px solid #e9ecef; padding-bottom: 10px;'>
                                {notification.Title}
                            </h2>
                            
                            <!-- Message -->
                            <div style='background: #f8f9fa; padding: 20px; border-left: 4px solid {priorityColor}; margin: 20px 0; border-radius: 0 5px 5px 0;'>
                                <p style='margin: 0; font-size: 16px; line-height: 1.5;'>{notification.Message}</p>
                            </div>
                            
                            {additionalInfo}
                            {actionButton}
                            
                            <!-- Details -->
                            <div style='background: #ffffff; border: 1px solid #e9ecef; border-radius: 5px; padding: 20px; margin: 20px 0;'>
                                <h3 style='margin: 0 0 15px 0; color: #495057; font-size: 16px;'>Notification Details</h3>
                                <table style='width: 100%; border-collapse: collapse;'>
                                    <tr>
                                        <td style='padding: 8px 0; border-bottom: 1px solid #f1f3f4; font-weight: bold; color: #6c757d; width: 30%;'>Type:</td>
                                        <td style='padding: 8px 0; border-bottom: 1px solid #f1f3f4;'>{type}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0; border-bottom: 1px solid #f1f3f4; font-weight: bold; color: #6c757d;'>Priority:</td>
                                        <td style='padding: 8px 0; border-bottom: 1px solid #f1f3f4;'>{notification.Priority}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0; border-bottom: 1px solid #f1f3f4; font-weight: bold; color: #6c757d;'>Created:</td>
                                        <td style='padding: 8px 0; border-bottom: 1px solid #f1f3f4;'>{notification.CreatedAt:MMM dd, yyyy 'at' HH:mm} UTC</td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0; font-weight: bold; color: #6c757d;'>ID:</td>
                                        <td style='padding: 8px 0; font-family: monospace; font-size: 12px; color: #6c757d;'>{notification.Id}</td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        
                        <!-- Footer -->
                        <div style='background: #f8f9fa; padding: 20px; text-align: center; border-top: 1px solid #e9ecef;'>
                            <p style='margin: 0; color: #6c757d; font-size: 12px;'>
                                This is an automated notification from <strong>Tasco System</strong>.<br>
                                If you have any questions, please contact your system administrator.
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private static string GenerateActionUrl(NotificationMessage notification)
        {
            // Generate URL based on notification type and metadata
            var baseUrl = "https://tasco.app"; // This should come from configuration
            
            return notification.Type switch
            {
                NotificationType.TaskStatusChanged or NotificationType.TaskAssigned or NotificationType.TaskCommentAdded 
                    when !string.IsNullOrWhiteSpace(notification.TaskId) 
                    => $"{baseUrl}/tasks/{notification.TaskId}",
                
                NotificationType.ProjectCreated or NotificationType.ProjectUpdated 
                    when !string.IsNullOrWhiteSpace(notification.ProjectId) 
                    => $"{baseUrl}/projects/{notification.ProjectId}",
                
                _ => $"{baseUrl}/notifications"
            };
        }

        private static string GenerateAdditionalInfo(NotificationMessage notification)
        {
            var additionalInfo = new List<string>();

            if (!string.IsNullOrWhiteSpace(notification.ProjectId))
            {
                additionalInfo.Add($"<strong>Project:</strong> {notification.ProjectId}");
            }

            if (!string.IsNullOrWhiteSpace(notification.TaskId))
            {
                additionalInfo.Add($"<strong>Task:</strong> {notification.TaskId}");
            }

            if (notification.Metadata != null && notification.Metadata.Any())
            {
                foreach (var metadata in notification.Metadata.Take(3)) // Show only first 3 metadata items
                {
                    if (metadata.Key != "email" && metadata.Key != "userEmail") // Skip email fields
                    {
                        additionalInfo.Add($"<strong>{metadata.Key}:</strong> {metadata.Value}");
                    }
                }
            }

            if (!additionalInfo.Any())
            {
                return "";
            }

            var infoItems = string.Join("<br>", additionalInfo);
            return $@"
                <div style='background: #e8f4fd; border: 1px solid #b8daff; border-radius: 5px; padding: 15px; margin: 20px 0;'>
                    <h4 style='margin: 0 0 10px 0; color: #004085; font-size: 14px;'>Additional Information</h4>
                    <p style='margin: 0; font-size: 14px; color: #004085;'>{infoItems}</p>
                </div>";
        }
    }
} 