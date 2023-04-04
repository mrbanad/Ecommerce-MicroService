namespace BasketService.RabbitMQ;

public class RabbitMqSettings
{
    public string HostName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string RetryCount { get; set; } = null!;
}