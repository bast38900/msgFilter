using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declare the exchange for all orders
channel.ExchangeDeclare(exchange: "orders", type: ExchangeType.Direct);

// Declare and bind the queues for widgets and gadgets
var widgetQueue = "widget";
var gadgetQueue = "gadget";

channel.QueueDeclare
(
  queue: widgetQueue, 
  durable: false, 
  exclusive: false, 
  autoDelete: false, 
  arguments: null
);

channel.QueueDeclare
(
  queue: gadgetQueue,
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

channel.QueueBind
(
  queue: gadgetQueue, 
  exchange: "orders", 
  routingKey: "gadget"
);

// Create a example order for a widget
var message = "Order for a gadget";
var body = Encoding.UTF8.GetBytes(message);

// Filter the message content and route it to the correct queue with the routing key
var routingKey = message.Contains("widget") ? "widget" : "gadget";
channel.BasicPublish(exchange: "orders", routingKey: routingKey, basicProperties: null, body: body);

Console.WriteLine($" [x] Sent '{message}'");
Console.ReadLine();

