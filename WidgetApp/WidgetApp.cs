using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declare the exchange for all orders if it doesn't already exist
channel.ExchangeDeclare
(
  exchange: "orders", 
  type: ExchangeType.Direct
);

// Declare and bind the queue for widgets
var widgetQueue = "widget";
channel.QueueDeclare
(
  queue: widgetQueue, 
  durable: false, 
  exclusive: false, 
  autoDelete: false, 
  arguments: null
);

channel.QueueBind
(
  queue: widgetQueue, 
  exchange: "orders", 
  routingKey: "widget"
);

Console.WriteLine("WidgetApp [*] Waiting for orders.");

// Create a consumer for the widget queue
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received '{message}'");
};

channel.BasicConsume
(
  queue: widgetQueue, 
  autoAck: true, 
  consumer: consumer
);

Console.ReadLine();

