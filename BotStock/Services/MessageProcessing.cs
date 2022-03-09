using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using System;

namespace BotStock.Services
{
    public class MessageProcessing : IMessageProcessing
    {
        private IProducer _producer;
        private IError _error;

        public MessageProcessing(IProducer producer, IError error)
        {
            _producer = producer;
            _error = error;

        }
        public void ProcessMessage(string message)
        {
            var messageParts = message.Split('=');

            switch (messageParts[0].ToLowerInvariant())
            {
                case "stock":
                    {
                        ProcessStockMessage(message);
                    }
                    break;
                default:
                    UnsuportedCommand(message);
                    break;
            }


        }

        public void ProcessStockMessage(string message)
        {
            try
            {
                var messageParts = message.Split('=');
                WebClient web = new WebClient();
                var a = web.DownloadString("https://stooq.com/q/l/?s=" + messageParts[1].ToLowerInvariant() + "&f=sd2t2ohlcv&h&e=csv");
                var stock = a.FromCsv<Stock>();
                _producer.Produce(stock.ToString());
            }
            catch (Exception ex)
            {
                _error.AddError(ex.Message);
            }


        }

        public void UnsuportedCommand(string message)
        {
            _error.AddError("UnsuportedCommand: " + message);
        }
    }
}
