using Microsoft.Extensions.Hosting;
using Tasco.NotificationService.Worker;
using Tasco.NotificationService.Worker.Consumers;
using Tasco.NotificationService.Worker.Messaging.Interface;
using Tasco.NotificationService.Worker.Messaging;
using Tasco.NotificationService.Worker.Services.Interfaces;
using Tasco.NotificationService.Worker.Services;
using Tasco.NotificationService.Worker.Channels.Interface;
using Tasco.NotificationService.Worker.Channels;
using Tasco.NotificationService.Service.Services;
using Tasco.NotificationService.Worker.SMTPs.Repositories;
using DotNetEnv;
using Microsoft.Extensions.Configuration;

// Load .env file
Env.Load();

var builder = Host.CreateApplicationBuilder(args);

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// Register dependencies
builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();

// Register notification channels
builder.Services.AddScoped<INotificationChannel, EmailNotificationChannel>();

// Add background workers
builder.Services.AddHostedService<NotificationMessageConsumer>(); // Consumer lắng nghe queue

var host = builder.Build();
host.Run();
