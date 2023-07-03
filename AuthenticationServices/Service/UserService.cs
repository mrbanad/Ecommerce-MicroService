using System.Linq.Expressions;
using System.Text;
using AuthenticationServices.Model;
using CommonClassLibrary.Dto.Authentication.User;
using CommonServiceLibrary.RabbitMQ;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace AuthenticationServices.Service;

public class UserService
{
    /// <summary>
    /// RabbitMqPersistentConnection
    /// </summary>
    private readonly IRabbitMqPersistentConnection _rabbitMqPersistentConnection;

    /// <summary>
    /// UserRepository
    /// </summary>
    private readonly Repository.UserRepository _userRepository;

    public UserService(Repository.UserRepository userRepository, IRabbitMqPersistentConnection rabbitMqPersistentConnection)
    {
        _userRepository = userRepository;
        _rabbitMqPersistentConnection = rabbitMqPersistentConnection;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAsync(int top, int skip, CancellationToken cancellationToken) =>
        await _userRepository.GetAsync(top, skip, cancellationToken);

    public async Task<ApplicationUser> GetAsync(string id, CancellationToken cancellationToken) =>
        await _userRepository.GetAsync(id, cancellationToken);

    public async Task<IEnumerable<ApplicationUser>> GetAsync(UserSearchDto condition, CancellationToken cancellationToken)
    {
        Expression<Func<ApplicationUser, bool>> predicate = p => true;

        if (!string.IsNullOrEmpty(condition.Title))
            predicate = p => p.UserName.Contains(condition.Title);

        if (!string.IsNullOrEmpty(condition.PhoneNumber))
            predicate = p => p.PhoneNumber == condition.PhoneNumber;

        return await _userRepository.GetAsync(predicate, condition.Top, condition.Skip, cancellationToken);
    }

    public async Task CreateAsync(ApplicationUser newUser, CancellationToken cancellationToken) =>
        await _userRepository.CreateAsync(newUser, cancellationToken);

    public Task UpdateAsync(ApplicationUser updatedUser, CancellationToken cancellationToken)
    {
        _rabbitMqPersistentConnection.TryConnect();

        var channel = _rabbitMqPersistentConnection.CreateModel();

        channel.ConfirmSelect();

        var json = JsonConvert.SerializeObject(updatedUser);
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "User",
            routingKey: string.Empty,
            basicProperties: null,
            body: body);

        channel.BasicAcks += async (sender, ea) => { await _userRepository.UpdateAsync(updatedUser, cancellationToken); };

        channel.BasicNacks += (sender, ea) => { Console.WriteLine(" error to send data to basket rabbitmq"); };

        return Task.CompletedTask;
    }

    public async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await _userRepository.RemoveAsync(id, cancellationToken);
}