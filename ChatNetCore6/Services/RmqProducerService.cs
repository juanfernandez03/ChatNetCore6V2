using System.Text;
using RabbitMQ.Client;

namespace ChatNetCore6.Services
{
    public class RmqProducerService : IRmqProducerService
    {
        private string _queueName;
        private IServiceProvider _serviceProvider;
        private ILogger<RmqConsumerService> _logger;
        private IConfiguration _config;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        public RmqProducerService(IServiceProvider serviceProvider, ILogger<RmqConsumerService> logger, IConfiguration config)
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

            _queueName = _config.GetValue<String>("QueueCommandsName");
            try
            {
                _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
            }
            catch
            {
                _logger.LogError("Queue already exists. Please choose another name or delete the queue");
                throw;
            }
        }

        public virtual void Produce(string message)
        {

            // Declare a RabbitMQ Queue

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
        }

    }
}
