using System.Linq.Expressions;
using System.Text;
using AuthenticationServices.Model;
using CommonClassLibrary.Dto.Authentication.Role;
using CommonServiceLibrary.RabbitMQ;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace AuthenticationServices.Service;

public class RoleService
{
    /// <summary>
    /// RabbitMqPersistentConnection
    /// </summary>
    private readonly IRabbitMqPersistentConnection _rabbitMqPersistentConnection;

    /// <summary>
    /// RoleRepository
    /// </summary>
    private readonly Repository.RoleRepository _roleRepository;

    public RoleService(Repository.RoleRepository roleRepository, IRabbitMqPersistentConnection rabbitMqPersistentConnection)
    {
        _roleRepository = roleRepository;
        _rabbitMqPersistentConnection = rabbitMqPersistentConnection;
    }

    public async Task<IEnumerable<ApplicationRole>> GetAsync(int top, int skip, CancellationToken cancellationToken) =>
        await _roleRepository.GetAsync(top, skip, cancellationToken);

    public async Task<ApplicationRole> GetAsync(string id, CancellationToken cancellationToken) =>
        await _roleRepository.GetAsync(id, cancellationToken);

    public async Task<IEnumerable<ApplicationRole>> GetAsync(RoleSearchDto condition, CancellationToken cancellationToken)
    {
        Expression<Func<ApplicationRole, bool>> predicate = p => true;

        if (!string.IsNullOrEmpty(condition.Title))
            predicate = p => p.Name.Contains(condition.Title);

        return await _roleRepository.GetAsync(predicate, condition.Top, condition.Skip, cancellationToken);
    }

    public async Task CreateAsync(ApplicationRole newRole, CancellationToken cancellationToken) =>
        await _roleRepository.CreateAsync(newRole, cancellationToken);

    public Task UpdateAsync(ApplicationRole updatedRole, CancellationToken cancellationToken)
    {
        _rabbitMqPersistentConnection.TryConnect();

        var channel = _rabbitMqPersistentConnection.CreateModel();

        channel.ConfirmSelect();

        var json = JsonConvert.SerializeObject(updatedRole);
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "Role",
            routingKey: string.Empty,
            basicProperties: null,
            body: body);

        channel.BasicAcks += async (sender, ea) => { await _roleRepository.UpdateAsync(updatedRole, cancellationToken); };

        channel.BasicNacks += (sender, ea) => { Console.WriteLine(" error to send data to basket rabbitmq"); };

        return Task.CompletedTask;
    }

    public async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await _roleRepository.RemoveAsync(id, cancellationToken);
}