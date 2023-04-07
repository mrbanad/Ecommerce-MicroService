using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Model;

namespace ProductService.Controllers;

[ApiController]
[Route("[action]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;
    private readonly Service.ProductService _productService;

    public HomeController(Service.ProductService productService, ILogger<HomeController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> AllProduct(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAsync(cancellationToken);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product, CancellationToken cancellationToken)
    {
        await _productService.CreateAsync(product, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ChangePrice(string productId, int price, CancellationToken cancellationToken)
    {
        var product = await _productService.GetAsync(productId, cancellationToken);

        product.Price = price;

        await _productService.UpdateAsync(product, cancellationToken);

        return Ok(product);
    }
}