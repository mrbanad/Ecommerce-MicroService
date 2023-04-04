using BasketService.Model;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.Controllers;

[ApiController]
[Route("[Action]")]
public class HomeController : ControllerBase
{
    private readonly Service.BasketService _basketService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(Service.BasketService basketService, ILogger<HomeController> logger)
    {
        _basketService = basketService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<Basket> Get(string objectId, CancellationToken cancellationToken)
    {
        var basket = await _basketService.GetAsync(objectId, cancellationToken);
        return basket;
    }
}