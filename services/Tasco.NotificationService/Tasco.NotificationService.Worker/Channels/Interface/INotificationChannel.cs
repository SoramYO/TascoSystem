using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasco.NotificationService.Worker.Models;

namespace Tasco.NotificationService.Worker.Channels.Interface
{
    public interface INotificationChannel
    {
        NotificationChannel ChannelType { get; }
        Task<ChannelResult> SendAsync(NotificationMessage notification, CancellationToken cancellationToken = default);
        bool CanHandle(NotificationChannel channel);
    }

    public interface IEmailNotificationChannel : INotificationChannel
    {
        Task<ChannelResult> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    }
}
