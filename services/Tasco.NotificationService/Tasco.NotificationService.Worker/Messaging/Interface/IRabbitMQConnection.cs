using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasco.NotificationService.Worker.Messaging.Interface
{
    public interface IRabbitMQConnection
    {
        IConnection GetConnection();
    }

}
