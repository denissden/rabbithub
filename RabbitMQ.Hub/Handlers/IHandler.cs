namespace RabbitMQ.Hub.Handlers;
public interface IHandler
{
  Task<IHandleResult> Handle(Message message);
}