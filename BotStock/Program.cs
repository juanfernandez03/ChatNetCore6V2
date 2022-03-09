using System;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Channels;
using RabbitMQ.Client.Events;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using BotStock.Services;


namespace BotStock
{
    class Send
    {
        public static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables("TPBChat_");
            var configuration = builder.Build();

            var hostName = configuration.GetValue<String>("RabbitMQHostname");
            var port = configuration.GetValue<int>("RabbitMQPort");
            var factory = new ConnectionFactory() { HostName = hostName, Port = port };

            var connectionProducer = factory.CreateConnection();
            var channelAnswers = connectionProducer.CreateModel();
            var queueAnswersName = configuration.GetValue<string>("QueueAnswersName");
            channelAnswers.QueueDeclare(queue: queueAnswersName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var connectionConsumer = factory.CreateConnection();
            var channelCommands = connectionConsumer.CreateModel();
            var queueCommandsName = configuration.GetValue<string>("QueueCommandsName");
            channelCommands.QueueDeclare(queue: queueAnswersName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var connectionError = factory.CreateConnection();
            var channelError = connectionConsumer.CreateModel();
            var queueErrorName = configuration.GetValue<string>("QueueErrorsName");
            channelError.QueueDeclare(queue: queueErrorName, durable: true, exclusive: false, autoDelete: false, arguments: null);


            var serviceProvider = new ServiceCollection()
                .AddTransient<IMessageProcessing, MessageProcessing>()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IProducer, Producer>((sp) =>
                {
                    return new Producer(channelAnswers, queueAnswersName);
                })
                 .AddSingleton<IError, Error>((sp) =>
                 {
                     return new Error(channelError, queueErrorName);
                 })
                .AddSingleton<IConsumer, Consumer>((sp) => {
                    return new Consumer(channelCommands, queueCommandsName, sp.GetService<IMessageProcessing>());
                })
                .BuildServiceProvider();



            serviceProvider.GetService<IConsumer>().Start();



            while (true)
            {
                var msg = Console.ReadLine();
                if (msg.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }

            channelAnswers.Dispose();
            connectionProducer.Dispose();
            channelCommands.Dispose();
            connectionConsumer.Dispose();
            channelError.Dispose();
            connectionError.Dispose();
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }
    }
}
