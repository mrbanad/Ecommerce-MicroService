using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;
using ProductService.Model;
using ProductService.RabbitMQ;
using RabbitMQ.Client;

namespace ProductService.Service;

public class ProductService
{
    private readonly Repository.ProductRepository _productRepository;
    private readonly IRabbitMqPersistentConnection _rabbitMqPersistentConnection;

    public ProductService(Repository.ProductRepository productRepository, IRabbitMqPersistentConnection rabbitMqPersistentConnection)
    {
        _productRepository = productRepository;
        _rabbitMqPersistentConnection = rabbitMqPersistentConnection;
    }

    public async Task<List<Product>> GetAsync(CancellationToken cancellationToken) =>
        await _productRepository.GetAsync(cancellationToken);

    public async Task<Product> GetAsync(string id, CancellationToken cancellationToken) =>
        await _productRepository.GetAsync(id, cancellationToken);

    public async Task<List<Product>> GetAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken) =>
        await _productRepository.GetAsync(predicate, cancellationToken);

    public async Task CreateAsync(Product newProduct, CancellationToken cancellationToken) =>
        await _productRepository.CreateAsync(newProduct, cancellationToken);

    public Task UpdateAsync(Product updatedProduct, CancellationToken cancellationToken)
    {
        _rabbitMqPersistentConnection.TryConnect();

        var channel = _rabbitMqPersistentConnection.CreateModel();

        channel.ConfirmSelect();

        var json = JsonConvert.SerializeObject(updatedProduct);
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "Product",
            routingKey: string.Empty,
            basicProperties: null,
            body: body);

        channel.BasicAcks += async (sender, ea) => { await _productRepository.UpdateAsync(updatedProduct.Id, updatedProduct, cancellationToken); };

        channel.BasicNacks += (sender, ea) => { Console.WriteLine(" error to send data to basket rabbitmq"); };

        return Task.CompletedTask;
    }

    public async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await _productRepository.RemoveAsync(id, cancellationToken);
}