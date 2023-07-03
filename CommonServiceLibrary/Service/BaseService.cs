using System.Linq.Expressions;
using System.Text;
using CommonClassLibrary.Dto;
using CommonClassLibrary.Model;
using CommonServiceLibrary.RabbitMQ;
using CommonServiceLibrary.Repository;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CommonServiceLibrary.Service;

public class BaseService<T> where T : IBaseModel
{
    private readonly IRabbitMqPersistentConnection _rabbitMqPersistentConnection;
    private readonly BaseRepository<T> _repository;

    protected BaseService(BaseRepository<T> repository, IRabbitMqPersistentConnection rabbitMqPersistentConnection)
    {
        _repository = repository;
        _rabbitMqPersistentConnection = rabbitMqPersistentConnection;
    }

    public virtual async Task<IEnumerable<T>> GetAsync(int top, int skip, CancellationToken cancellationToken) =>
        await _repository.GetAsync(top, skip, cancellationToken);

    public virtual async Task<T> GetAsync(string id, CancellationToken cancellationToken) =>
        await _repository.GetAsync(id, cancellationToken);

    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, BaseSearchDto condition, CancellationToken cancellationToken)
    {
        predicate = BasePredicate(predicate, condition);

        return await _repository.GetAsync(predicate, condition.Top, condition.Skip, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, BaseSearchWithDisplayDto condition, CancellationToken cancellationToken)
    {
        predicate = BasePredicate(predicate, condition);

        return await _repository.GetAsync(predicate, condition.Top, condition.Skip, cancellationToken);
    }

    public virtual async Task CreateAsync(T newItem, CancellationToken cancellationToken) =>
        await _repository.CreateAsync(newItem, cancellationToken);

    public virtual async Task UpdateAsync(T updatedItem, CancellationToken cancellationToken)
    {
        _rabbitMqPersistentConnection.TryConnect();

        var channel = _rabbitMqPersistentConnection.CreateModel();

        channel.ConfirmSelect();

        var json = JsonConvert.SerializeObject(updatedItem);
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: typeof(T).Name,
            routingKey: string.Empty,
            basicProperties: null,
            body: body);

        channel.BasicAcks += async (sender, ea) => { await _repository.UpdateAsync(updatedItem, cancellationToken); };

        channel.BasicNacks += (sender, ea) => { Console.WriteLine("Error sending data to RabbitMQ"); };

        await Task.CompletedTask;
    }

    public virtual async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await _repository.RemoveAsync(id, cancellationToken);

    #region Private

    private static Expression<Func<T, bool>> BasePredicate(Expression<Func<T, bool>> predicate, IBaseSearchDto condition)
    {
        if (condition.Action != null)
            predicate = p => p.Active == condition.Action;

        if (!string.IsNullOrEmpty(condition.Creator))
            predicate = p => p.Creator.Contains(condition.Creator);

        if (!string.IsNullOrEmpty(condition.Editor))
            predicate = p => p.Editor != null && p.Editor.Contains(condition.Editor);

        if (condition.StartDate != null)
            predicate = p => p.CreateDate > condition.StartDate.Value;

        if (condition.EndDate != null)
            predicate = p => p.CreateDate < condition.EndDate.Value;

        return predicate;
    }

    #endregion
}