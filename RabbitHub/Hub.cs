using System.Collections.Concurrent;
using RabbitMQ.Client;
using RabbitHub.Config;

namespace RabbitHub;


public record DuplexModel(IModel Send, IModel Receive);
public partial class Hub
{
  public string AppId => connConf?.AppId ?? "";
  private ConnectionConfig connConf;
  private IConnection connection;
  private IModel messageChannel;
  private DuplexModel rpcChannel;
  private List<IModel> channels = new();
  private IBasicConsumer rpcCallbackConsumer;
  private string callbackQueue;
  private string callbackTopic => callbackQueue + ".topic";

  private ConcurrentDictionary<UlongAndGuid, TaskCompletionSource<Message?>> rpcWaitingCallback = new();

  public record struct UlongAndGuid(ulong number, Guid guid) : IEquatable<UlongAndGuid>
  {
    public bool Equals(UlongAndGuid? other)
    {
      return other != null && (number == other.Value.number || guid == other.Value.guid);
    }
  }

#nullable disable
  public Hub(ConnectionConfig connectionConfig)
  {
    connConf = connectionConfig;
  }
#nullable enable
}
