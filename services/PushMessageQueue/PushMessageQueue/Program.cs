using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using PushMessageQueue;

await Main();

static async Task Main()
{
    const string queueName = "notification_queue";

    // 🔧 Connection configuration - sửa port thành 5673
    var factory = new ConnectionFactory()
    {
        HostName = "localhost",
        Port = 5673,                  // ✅ Sửa từ 5672 thành 5673
        UserName = "admin",           // ✅ Đúng với Docker Compose
        Password = "admin123",        // ✅ Đúng với Docker Compose  
        VirtualHost = "/",
        RequestedHeartbeat = TimeSpan.FromSeconds(60),
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        AutomaticRecoveryEnabled = true
    };

    // 🔄 Retry logic để kết nối
    const int maxRetries = 5;
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            Console.WriteLine($"🔄 Attempt {attempt}/{maxRetries}: Connecting to RabbitMQ...");
            Console.WriteLine($"   Host: {factory.HostName}:{factory.Port}");
            Console.WriteLine($"   User: {factory.UserName}");

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            Console.WriteLine("✅ Connected to RabbitMQ successfully!");

            // Test connection bằng cách declare queue
            await channel.QueueDeclareAsync(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine($"✅ Queue '{queueName}' declared successfully!");

            // 📨 Send test messages
            await SendTestMessages(channel, queueName);

            Console.WriteLine("🎉 All messages sent successfully!");
            Console.WriteLine("📧 Check email at: tquang0809@gmail.com");
            Console.WriteLine("🌐 RabbitMQ Management UI: http://localhost:15672");
            return; // Exit successfully
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Attempt {attempt} failed: {ex.Message}");

            if (attempt == maxRetries)
            {
                Console.WriteLine("\n💡 Troubleshooting suggestions:");
                Console.WriteLine("1. Make sure Docker containers are running:");
                Console.WriteLine("   docker-compose ps");
                Console.WriteLine("\n2. Check RabbitMQ container logs:");
                Console.WriteLine("   docker-compose logs rabbitmq");
                Console.WriteLine("\n3. Verify RabbitMQ Management UI:");
                Console.WriteLine("   http://localhost:15672 (admin/admin123)");
                Console.WriteLine("\n4. Check if notification-service is consuming:");
                Console.WriteLine("   docker-compose logs notification-service");
                Environment.Exit(1);
            }

            Console.WriteLine($"⏳ Waiting 3 seconds before retry...\n");
            await Task.Delay(3000);
        }
    }
}

static async Task SendTestMessages(IChannel channel, string queueName)
{
    var messages = new List<NotificationMessage>
    {
        // 📋 Task Assigned
        new NotificationMessage
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "ngoxuanson121@gmail.com",
            Title = "🎯 Task mới: Phát triển API Authentication",
            Message = "Chào Duy Quang! Bạn đã được giao task phát triển API Authentication cho hệ thống Tasco. Deadline: 30/01/2025. Chúc bạn coding vui vẻ! 🚀",
            Type = NotificationType.TaskAssigned,
            ProjectId = "tasco-auth-system",
            TaskId = "task-auth-api-001",
            Priority = NotificationPriority.High,
            Channels = new List<NotificationChannel> { NotificationChannel.Email },
            CreatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                { "assignee", "Duy Quang" },
                { "reporter", "Team Lead - Nguyễn Văn A" },
                { "deadline", "2025-01-30T17:00:00Z" },
                { "status", "To Do" },
                { "email", "ngoxuanson121@gmail.com" },
                { "estimatedHours", "32" },
                { "tags", "backend,authentication,security" }
            }
        },

        // ⏰ Deadline Reminder  
        new NotificationMessage
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "ngoxuanson121@gmail.com",
            Title = "⏰ Nhắc nhở: Task sắp deadline!",
            Message = "Hi Duy Quang! Task \"Database Migration & Optimization\" sẽ đến deadline trong 2 ngày nữa (25/01/2025). Progress hiện tại: 75%. Keep going! 💪",
            Type = NotificationType.DeadlineReminder,
            ProjectId = "tasco-db-optimization",
            TaskId = "task-db-migration-002",
            Priority = NotificationPriority.Critical,
            Channels = new List<NotificationChannel> { NotificationChannel.Email },
            CreatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                { "assignee", "Duy Quang" },
                { "deadline", "2025-01-25T18:00:00Z" },
                { "progress", "75%" },
                { "daysLeft", "2" },
                { "email", "ngoxuanson121@gmail.com" },
                { "status", "In Progress" },
                { "blockers", "Waiting for DBA review" }
            }
        },

        // 💬 Comment Added
        new NotificationMessage
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "ngoxuanson121@gmail.com",
            Title = "💬 Bình luận mới trên task của bạn",
            Message = "Senior Developer vừa comment: \"Code architecture looks solid! Just need to add more unit tests for edge cases. Overall great work Quang! 👏\"",
            Type = NotificationType.TaskCommentAdded,
            ProjectId = "tasco-code-review",
            TaskId = "task-unit-testing-003",
            Priority = NotificationPriority.Normal,
            Channels = new List<NotificationChannel> { NotificationChannel.Email },
            CreatedAt = DateTime.UtcNow,
            Metadata = new Dictionary<string, object>
            {
                { "assignee", "Duy Quang" },
                { "author", "Senior Dev - Trần Văn B" },
                { "comment", "Code architecture looks solid! Just need to add more unit tests for edge cases. Overall great work Quang! 👏" },
                { "commentId", "comment-" + Guid.NewGuid().ToString()[..8] },
                { "email", "ngoxuanson121@gmail.com" },
                { "status", "In Review" },
                { "reviewScore", "8/10" }
            }
        }
    };

    foreach (var message in messages)
    {
        await PublishMessageAsync(channel, queueName, message);
        await Task.Delay(1000); // Delay giữa các messages
    }
}

static async Task PublishMessageAsync(IChannel channel, string queueName, NotificationMessage message)
{
    try
    {
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true,
            MessageId = message.Id,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
            basicProperties: properties,
            body: body);

        Console.WriteLine($"✅ Message sent: {message.Type}");
        Console.WriteLine($"   📧 To: {message.UserId}");
        Console.WriteLine($"   🏷️  Priority: {message.Priority}");
        Console.WriteLine($"   📝 Title: {message.Title}");
        Console.WriteLine("─────────────────────────────────────");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to send message: {ex.Message}");
    }
}