using CommonServiceLibrary.RabbitMQ;

namespace CategoryServices.RabbitMQ;

public class RabbitMqEvent : IHostedService
{
    private readonly IRabbitMqPersistentConnection _rabbitMqPersistentConnection;

    public RabbitMqEvent(IRabbitMqPersistentConnection rabbitMqPersistentConnection)
    {
        _rabbitMqPersistentConnection = rabbitMqPersistentConnection;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}