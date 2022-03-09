using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace BotStock.Services
{
    public class Producer : IProducer
    {
        private IModel _channelAnswers;
        private string _queueAnswersName;

        public Producer(IModel channelAnswers, string queueAnswersName)
        {
            _queueAnswersName = queueAnswersName;
            _channelAnswers = channelAnswers;
        }

        public void Start()
        {

        }

        public void Produce(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channelAnswers.BasicPublish(exchange: "", routingKey: _queueAnswersName, basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }
    }
}
