using System.Security.Claims;
using ChatNetCore6.Data;
using ChatNetCore6.Models;
using ChatNetCore6.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatNetCore6.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly IRmqProducerService _producer;

        public ChatHub(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext, IRmqProducerService producer)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _producer = producer;
        }
        public async Task SendMessage(Message message)
        {
            if (message.Text.StartsWith('/'))
            {
                var messageCommand = message.Text.Split('/');

                if (messageCommand.Length != 2)
                {
                    throw new Exception("Wrong message format");
                }
                await Clients.All.SendAsync("receiveMessage",message );
                _producer.Produce(messageCommand[1]);
                return;
            }

            //_dbContext.Messages.Add(new Message
            //{
            //    Body = message,
            //    UserName = userName,
            //    TimeStamp = DateTime.Now
            //});
            //_dbContext.SaveChanges();

            await Clients.All.SendAsync("receiveMessage", message);
        }
        //public async Task SendMessage(Message message) =>
        //    await Clients.All.SendAsync("receiveMessage", message);
    }
}
