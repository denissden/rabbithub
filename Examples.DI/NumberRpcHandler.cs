using RabbitHub;
using RabbitHub.Handlers;

namespace Examples.DI;

public record NumberResult(int Value, bool IsEven);

public class NumberRpcHandler : RpcHandler<NumberResult>
{
  private readonly Random random = new();
  public override async Task<IHandleResult> Handle(Message message)
  {
    Console.WriteLine($"Message: {message.Text} ({message.CorrelationId})");
    await Task.Delay(random.Next(10, 1000));
    var body = message.Text;
    int num = Convert.ToInt32(body);

    return Ack(new NumberResult(num, num % 2 == 0));
  }
}
