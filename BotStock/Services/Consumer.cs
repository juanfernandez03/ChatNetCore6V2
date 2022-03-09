using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BotStock.Services
{
    public class Consumer : IConsumer
    {
        private IModel _channelCommands;
        private string _queueConsumerName;
        private IMessageProcessing _messageProcessing;

        public Consumer(IModel channel, string queueConsumerName, IMessageProcessing messageProcessing)
        {
            _channelCommands = channel;
            _queueConsumerName = queueConsumerName;
            _messageProcessing = messageProcessing;
        }

        public void Start()
        {
            _channelCommands.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(_channelCommands);
            consumer.Received += this.Consume;
            _channelCommands.BasicConsume(queue: _queueConsumerName, autoAck: false, consumer: consumer);
        }


        private void Consume(object? model, BasicDeliverEventArgs ea)
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine(" [x] Received {0}", message);
            _messageProcessing.ProcessMessage(message);
            Console.WriteLine(" [x] Done");

            _channelCommands.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}
