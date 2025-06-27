using System.Threading.Tasks;
using Tasco.TaskService.Repository.Entities;

namespace Tasco.TaskService.Service.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(NotificationMessage message);
    }
}