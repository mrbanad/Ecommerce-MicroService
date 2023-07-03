using CategoryServices.Model;
using CategoryServices.Repository;
using CommonClassLibrary.Dto.Category;
using CommonServiceLibrary.RabbitMQ;
using CommonServiceLibrary.Service;

namespace CategoryServices.Service;

public class CategoryService : BaseService<Category>
{
    /// <summary>
    /// CategoryRepository
    /// </summary>
    private readonly CategoryRepository _repository;

    public CategoryService(CategoryRepository repository, IRabbitMqPersistentConnection rabbitMqPersistentConnection)
        : base(repository, rabbitMqPersistentConnection)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Category>> GetAsync(CategorySearchDto condition, CancellationToken cancellationToken)
    {
        var predicate = DisplayService<Category>.BasePredicate(condition);

        return await _repository.GetAsync(predicate, condition.Top, condition.Skip, cancellationToken);
    }
}