using RabbitMQ.Client;
using RabbitHub.Extensions;
using System.Text.Json;

namespace RabbitHub;
public partial class Hub
{
  public void Message(object @object, string topic)
  {
    var bytes = JsonSerializer.SerializeToUtf8Bytes(@object);
    var message = new Message(bytes);
    Message(message, topic);
  }

  public void Message(Message message, string topic)
  {
    var props = rpcChannel.Send.CreateBasicProperties();
    props.FillFromMessage(message);
    messageChannel.BasicPublish(
      exchange: connConf.Exchange,
      routingKey: message.Topic,
      body: message.Body,
      basicProperties: props);
  }

  public async Task<T?> Rpc<T>(object @object, string topic)
  {
    var bytes = JsonSerializer.SerializeToUtf8Bytes(@object);
    var message = new Message(bytes);
    var response = await Rpc(message, topic);
    if (response is null)
      return default(T);
    return response.GetJsonContent<T>();
  }

  public async Task<Message?> Rpc(Message message, string topic)
  {
    if (!string.IsNullOrEmpty(topic))
    {
      message.Topic = topic;
    }
    message.IsRpc = true;

    var tcs = new TaskCompletionSource<Message?>();

    // make sure correlation id is unique
    ulong nextSeqNo = rpcChannel.Send.NextPublishSeqNo;
    Guid correlationId;
    do { correlationId = Guid.NewGuid(); }
    while (!rpcWaitingCallback.TryAdd(new UlongAndGuid(nextSeqNo, correlationId), tcs));

    var props = rpcChannel.Send.CreateBasicProperties();
    props.Headers = message.Headers;
    props.CorrelationId = correlationId.ToString();


    // TODO: publisher confirms
    // [x] nacks
    // [ ] acks
    // https://github.com/rabbitmq/rabbitmq-tutorials/blob/master/dotnet/PublisherConfirms/PublisherConfirms.cs

    var replyAddress = new PublicationAddress(
      //  IMPORTANT: exchange type must be at least one character 
      // long because of internal PublicationAddress regex. 
      //  It tries to resolve exchange type  using "([^:]+)" 
      // so regex fails to match with an empty string type. 
      //  The variable is not used anywhere internally 
      exchangeType: connConf.Exchange.ToString(),
      exchangeName: connConf.Exchange,
      routingKey: callbackTopic);

    props.ReplyToAddress = replyAddress;

    rpcChannel.Send.BasicPublish(
      exchange: connConf.Exchange,
      routingKey: message.Topic,
      body: message.Body,
      basicProperties: props,
      mandatory: true);
    var rpcTask = tcs.Task;

    // TODO: timeout
    return await rpcTask.ConfigureAwait(false);
  }
}

