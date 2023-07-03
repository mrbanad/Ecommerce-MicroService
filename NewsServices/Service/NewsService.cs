using CommonClassLibrary.Dto.News;
using CommonServiceLibrary.RabbitMQ;
using CommonServiceLibrary.Service;
using NewsServices.Model;
using NewsServices.Repository;

namespace NewsServices.Service;

public class NewsService : BaseService<News>
{
    /// <summary>
    /// NewsRepository
    /// </summary>
    private readonly NewsRepository _repository;

    public NewsService(NewsRepository repository, IRabbitMqPersistentConnection rabbitMqPersistentConnection)
        : base(repository, rabbitMqPersistentConnection)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<News>> GetAsync(NewsSearchDto condition, CancellationToken cancellationToken)
    {
        var predicate = DisplayService<News>.BasePredicate(condition);

        return await _repository.GetAsync(predicate, condition.Top, condition.Skip, cancellationToken);
    }
}