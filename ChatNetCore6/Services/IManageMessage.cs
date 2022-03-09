using ChatNetCore6.Models;

namespace ChatNetCore6.Services
{
    public interface IManageMessage
    {
        List<Message> GetMessages();
        Message CreateNewMessage(Message message);
    }
}
