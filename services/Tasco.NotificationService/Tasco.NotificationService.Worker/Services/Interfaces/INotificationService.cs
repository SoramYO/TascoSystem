using Tasco.NotificationService.Worker.Models;

namespace Tasco.NotificationService.Worker.Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResult> SendNotificationAsync(NotificationMessage notification, CancellationToken cancellationToken = default);
    }
} 