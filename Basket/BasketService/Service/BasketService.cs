using System.Linq.Expressions;
using BasketService.Model;

namespace BasketService.Service;

public class BasketService
{
    private readonly Repository.BasketRepository _basketRepository;

    public BasketService(Repository.BasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<List<Basket>> GetAsync(CancellationToken cancellationToken) =>
        await _basketRepository.GetAsync(cancellationToken);

    public async Task<Basket> GetAsync(string id, CancellationToken cancellationToken) =>
        await _basketRepository.GetAsync(id, cancellationToken);

    public async Task<List<Basket>> GetAsync(Expression<Func<Basket, bool>> predicate, CancellationToken cancellationToken) =>
        await _basketRepository.GetAsync(predicate, cancellationToken);

    public async Task CreateAsync(Basket newBasket, CancellationToken cancellationToken) =>
        await _basketRepository.CreateAsync(newBasket, cancellationToken);

    public async Task UpdateAsync(Basket updatedBasket, CancellationToken cancellationToken) =>
        await _basketRepository.UpdateAsync(updatedBasket.Id, updatedBasket, cancellationToken);

    public async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
        await _basketRepository.RemoveAsync(id, cancellationToken);
}