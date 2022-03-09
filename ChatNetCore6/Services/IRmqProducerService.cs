namespace ChatNetCore6.Services
{
    public interface IRmqProducerService
    {
        void Produce(string message);
    }
}
