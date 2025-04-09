public interface IMessagePublisher
{
    void Send(string topic, string message);
}
