using System.Linq.Expressions;
using CommentServices.Model;
using CommentServices.Repository;
using CommonClassLibrary.Dto.Comment;
using CommonServiceLibrary.RabbitMQ;
using CommonServiceLibrary.Service;

namespace CommentServices.Service;

public class CommentService : BaseService<Comment>
{
    /// <summary>
    /// CommentRepository
    /// </summary>
    private readonly CommentRepository _repository;

    public CommentService(CommentRepository repository, IRabbitMqPersistentConnection rabbitMqPersistentConnection)
        : base(repository, rabbitMqPersistentConnection)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Comment>> GetAsync(CommentSearchDto condition, CancellationToken cancellationToken)
    {
        Expression<Func<Comment, bool>> predicate = p => true;

        if (!string.IsNullOrEmpty(condition.Text))
            predicate = p => p.Text.Contains(condition.Text);

        return await _repository.GetAsync(predicate, condition.Top, condition.Skip, cancellationToken);
    }
}