using System.Text;
using ChatNetCore6.Hubs;
using ChatNetCore6.Models;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ChatNetCore6.Services
{
    // RabbitMQService.cs
    public class RmqConsumerService : IRmqConsumerService
    {
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;

        protected readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RmqConsumerService> _logger;
        private readonly IConfiguration _config;

        public RmqConsumerService(IServiceProvider serviceProvider, ILogger<RmqConsumerService> logger, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _config = config;

            var hostName = _config.GetValue<String>("RabbitMQHostname");
            var port = _config.GetValue<int>("RabbitMQPort");
            // Opens the connections to RabbitMQ
            _factory = new ConnectionFactory() { HostName = hostName, Port = port };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();


        }

        public virtual void Connect()
        {
            var queueName = _config.GetValue<String>("QueueAnswersName");
            try
            {
                _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            }
            catch
            {
                _logger.LogError("Queue already exists. Please choose another name or delete the queue");
                throw;
            }
            // Declare a RabbitMQ Queue


            var consumer = new EventingBasicConsumer(_channel);

            // Consume a RabbitMQ Queue
            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            // When we receive a message from rabbitmq

            consumer.Received += delegate (object model, BasicDeliverEventArgs ea)
            {
                // Get the ChatHub from SignalR (using DI)
                var chatHub = (IHubContext<ChatHub>)_serviceProvider.GetService(typeof(IHubContext<ChatHub>));



                byte[] body = ea.Body.ToArray();
                var stock = Encoding.UTF8.GetString(body);
                Message message = CreateMessage(stock);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                chatHub.Clients.All.SendAsync("receiveMessage",message);

            };


        }

        private static Message CreateMessage(string stock)
        {
            Message message = new Message();
            message.Text = stock;
            message.When = DateTime.Now;
            message.UserName = "BOT";
            return message;
        }
    }

}

