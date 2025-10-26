
using RabbitMQ.Client;
using System.Text;

namespace MyProject.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQPublisher
    {
        public void Publish(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "demo-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "demo-queue", basicProperties: null, body: body);
        }
    }
}
