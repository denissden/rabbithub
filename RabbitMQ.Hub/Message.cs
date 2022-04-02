using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace RabbitMQ.Hub;

public class Message
{
  public string? Topic { get; set; } = "";
  public ReadOnlyMemory<byte> Body { get; set; }
  public string Text
  {
    get => Encoding.UTF8.GetString(Body.ToArray());
    set => Body = Encoding.UTF8.GetBytes(value);
  }
  public ulong DeliveryTag { get; set; }
  public string? CorrelationId { get; set; }
  public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();
  public bool IsRpc
  {
    get
    {
      object? value = default;
      Headers?.TryGetValue("isRpc", out value);
      return (value as bool?) == true;
    }
    set
    {
      // if (Headers.ContainsKey("isRpc"))
      // {
      //   Headers["isRpc"] = value;
      // }
      // else
      // {
      //   Headers.Add("isRpc", value);
      // }
    }
  }

  public Message() { }

  public Message(ReadOnlyMemory<byte> content)
  {
    Body = content;
  }

  public Message(string content)
  {
    Text = content;
  }
}

public static class MessageExtensions
{
  public static Message FillHandlerProperties(
    this Message message,
    string consumerTag,
    ulong deliveryTag,
    bool redelivered,
    string exchange,
    string routingKey,
    IBasicProperties properties,
    ReadOnlyMemory<byte> body)
  {
    message.Topic = routingKey;
    // TODO: fix memory copying
    // set message body to a copy of original ReadOnlyMemory<byte> body
    // because original body for some reason gets deleted by garbage collector
    // for some reason no memory exceptions are thrown when trying to access
    // an already deleted message body
    message.Body = body.ToArray();
    message.DeliveryTag = deliveryTag;
    message.CorrelationId = properties.CorrelationId;
    message.Headers = properties.Headers;

    return message;
  }

  public static Message FillEventArgs(this Message message, BasicDeliverEventArgs args)
  {
    return message.FillHandlerProperties(
      args.ConsumerTag,
      args.DeliveryTag,
      args.Redelivered,
      args.Exchange,
      args.RoutingKey,
      args.BasicProperties,
      args.Body
    );
  }

  public static void FillBasicProps(this Message message, IBasicProperties props)
  {
    props.Headers = message.Headers;
    props.CorrelationId = message.CorrelationId;
  }

  public static T? GetJsonContent<T>(this Message message)
  {
    return JsonSerializer.Deserialize<T>(message.Text);
  }
}