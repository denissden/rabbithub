using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Hub;
public partial class RabbitHub
{
  public void Message(Message message, string topic)
  {
    var props = rpcChannel.Send.CreateBasicProperties();
    message.FillBasicProps(props);
    messageChannel.BasicPublish(
      exchange: connConf.Exchange,
      routingKey: message.Topic,
      body: message.Body,
      basicProperties: props);
  }

  public async Task<Message> Rpc(Message message, string topic)
  {
    if (!string.IsNullOrEmpty(topic))
    {
      message.Topic = topic;
    }
    message.IsRpc = true;

    var tcs = new TaskCompletionSource<Message>();

    // make sure correlation id is unique
    Guid correlationId;
    do { correlationId = Guid.NewGuid(); }
    while (!rpcWaitingCallback.TryAdd(correlationId, tcs));

    var props = rpcChannel.Send.CreateBasicProperties();
    props.Headers = message.Headers;
    props.CorrelationId = correlationId.ToString();


    // TODO: publisher confirms
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
      basicProperties: props);
    var rpcTask = tcs.Task;
    return await rpcTask.ConfigureAwait(false);
  }
}

