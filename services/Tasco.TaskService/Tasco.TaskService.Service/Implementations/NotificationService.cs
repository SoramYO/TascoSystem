using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Impl;
using System.Text;
using System.Threading.Tasks;
using Tasco.TaskService.Repository.Entities;
using Tasco.TaskService.Service.Interfaces;

namespace Tasco.TaskService.Service.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly ConnectionFactory _factory;

        public NotificationService()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5673,
                UserName = "admin",
                Password = "admin123",
                VirtualHost = "/"
            };
        }

        public async Task SendNotificationAsync(NotificationMessage message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var queueName = "notification_queue";
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                MessageId = message.Id,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body
            );
            await Task.CompletedTask;
        }
    }
}