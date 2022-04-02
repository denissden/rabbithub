using RabbitMQ.Hub;
using RabbitMQ.Hub.Config;
using RabbitMQ.Hub.Extensions;

public class Program
{
  public static string Topic = "test.number.rpc";
  public static async Task Main(string[] args)
  {
    if (args.Length == 0)
    {
      Console.WriteLine("No 'send' or 'receive' arg!");
      return;
    }

    var conf = ConnectionConfig.GetDefault("test");

    var hub = RabbitHub.Connect(conf);

    var arg = args[0];
    if (arg == "send")
    {
      string repeatsStr = "1";
      if (args.Length >= 2) repeatsStr = args[1];
      if (int.TryParse(repeatsStr, out int repeats))
      {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var tasks = new List<Task<Message>>();
        Console.WriteLine($"Sending {repeats} messages");
        for (int i = 0; i < repeats; i++)
        {
          var responseTask = hub.Rpc(new Message() { Text = $"{i}" }, topic: Topic);
          tasks.Add(responseTask);
        }

        var res = await Task.WhenAll(tasks);

        watch.Stop();

        Console.WriteLine("Received all");
        foreach (var r in res)
        {
          Console.WriteLine($"Message: {r.Text}");
        }

        Console.WriteLine($"Time taken: {watch.ElapsedMilliseconds}ms");
      }
      else
      {
        Console.WriteLine($"Parameter must be a number not {repeatsStr}");
      }
    }

    else if (arg == "receive")
    {
      var consumer = new DefaultConsumer();

      var random = new Random();
      consumer.Handle(Topic, async (msg, handler) =>
      {
        Console.WriteLine($"Message: {msg.Text} ({msg.CorrelationId})");
        await Task.Delay(random.Next(10, 1000));
        var body = msg.Text;
        int num = Convert.ToInt32(body);

        return handler.Ack(new { Value = num, IsEven = num % 2 == 0 });
      });
      hub.Consume(consumer, QueueConfig.Create("q_test"), declareQueue: true, bindTopics: true);

      Console.ReadLine();
    }
  }

  public record Num(int Value);
}
