using ChatNetCore6.Data;
using ChatNetCore6.Models;

namespace ChatNetCore6.Services
{
    public class ManageMessage : IManageMessage
    {
        private readonly ApplicationDbContext _dbContext;

        public ManageMessage(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Message CreateNewMessage(Message message)
        {
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
            return message;
        }

        public List<Message> GetMessages()
        {
            return _dbContext.Messages.OrderByDescending(x => x.When).Take(50).ToList();
        }
    }
}
