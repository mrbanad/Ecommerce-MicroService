using ProductService.RabbitMQ;

namespace ApiGateway.RabbitMQ;

public class RabbitMqEvent : IHostedService
{
    private readonly IRabbitMqPersistentConnection _rabbitMqPersistentConnection;

    public RabbitMqEvent(IRabbitMqPersistentConnection rabbitMqPersistentConnection)
    {
        _rabbitMqPersistentConnection = rabbitMqPersistentConnection;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _rabbitMqPersistentConnection.TryConnect();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}