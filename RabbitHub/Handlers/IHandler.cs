namespace RabbitHub.Handlers;
public interface IHandler
{
  Task<IHandleResult> Handle(Message message);
}