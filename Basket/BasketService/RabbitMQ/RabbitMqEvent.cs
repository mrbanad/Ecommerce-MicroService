using System.Text;
using BasketService.Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BasketService.RabbitMQ;

public class RabbitMqEvent : IHostedService
{
    private readonly Service.BasketService _basketService;
    private readonly IRabbitMqPersistentConnection _rabbitMqPersistentConnection;

    public RabbitMqEvent(IRabbitMqPersistentConnection rabbitMqPersistentConnection, Service.BasketService basketService)
    {
        _rabbitMqPersistentConnection = rabbitMqPersistentConnection;
        _basketService = basketService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _rabbitMqPersistentConnection.TryConnect();
        var channel = _rabbitMqPersistentConnection.CreateModel();

        var queueName = channel.QueueDeclare().QueueName;

        while (true)
        {
            try
            {
                channel.ExchangeDeclarePassive("Product");
                break;
            }
            catch (Exception ex)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        channel.QueueBind(queue: queueName,
            exchange: "Product",
            routingKey: string.Empty);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var product = JsonConvert.DeserializeObject<Product>(json);

            var baskets = await _basketService.GetAsync(x => x.Items.Any(y => y.Id == product.Id), cancellationToken);

            foreach (var item in baskets.SelectMany(x => x.Items))
            {
                if (item.Id != product.Id) continue;

                item.Title = product.Title;
                item.Price = product.Price;
            }

            foreach (var basket in baskets)
            {
                await _basketService.UpdateAsync(basket, cancellationToken);
            }
        };

        channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}