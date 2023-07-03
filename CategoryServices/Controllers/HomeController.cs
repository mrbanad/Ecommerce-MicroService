using CategoryServices.Model;
using CategoryServices.Service;
using CommonClassLibrary.Dto.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CategoryServices.Controllers;

/// <summary>
/// CategoryController
/// </summary>
[ApiController]
[Route("[action]")]
public class HomeController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// CategoryController
    /// </summary>
    /// <param name="categoryService"></param>
    /// <param name="logger"></param>
    public HomeController(Service.CategoryService categoryService, ILogger<HomeController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Get All Category For AdminPanel
    /// </summary>
    /// <param name="top"></param>
    /// <param name="skip"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>All Category</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int top, int skip, CancellationToken cancellationToken)
    {
        var categorys = await _categoryService.GetAsync(top, skip, cancellationToken);
        return Ok(categorys);
    }

    /// <summary>
    /// Category With Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Category</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetAsync(id, cancellationToken);
        return Ok(category);
    }

    /// <summary>
    /// Get Category With Condition
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>All Active Category</returns>
    [HttpPost]
    public async Task<IActionResult> Get([FromBody] CategorySearchDto condition, CancellationToken cancellationToken)
    {
        var categorys = await _categoryService.GetAsync(condition, cancellationToken);
        return Ok(categorys);
    }

    /// <summary>
    /// Create Category
    /// </summary>
    /// <param name="category"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>StatusCode:200</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpPut]
    public async Task<IActionResult> Create([FromBody] Category category, CancellationToken cancellationToken)
    {
        await _categoryService.CreateAsync(category, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Update Category
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="category"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated Category</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] Category category, CancellationToken cancellationToken)
    {
        await _categoryService.UpdateAsync(category, cancellationToken);
        var result = await _categoryService.GetAsync(category.Id, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Delete Category
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>StatusCode:200</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpDelete]
    public async Task<IActionResult> Delete(string categoryId, CancellationToken cancellationToken)
    {
        await _categoryService.RemoveAsync(categoryId, cancellationToken);
        return Ok();
    }
}