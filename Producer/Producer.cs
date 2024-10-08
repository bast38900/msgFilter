using System;
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

// Create an example order
var message = "gadget:1, widget:12, stuff:43";

// Split the message and send relevant order to correct consumer
var orderItem = message.Split(", ");
foreach (var item in orderItem)
{
    var routingKey = item.Contains("widget") ? "widget" : item.Contains("gadget") ? "gadget" : null;
    if (routingKey != null)
    {
        var body = Encoding.UTF8.GetBytes(item);
        channel.BasicPublish(exchange: "orders", routingKey: routingKey, basicProperties: null, body: body);
        Console.WriteLine($" [x] Sent '{item}' to '{routingKey}' queue");
    }
}

Console.ReadLine();
