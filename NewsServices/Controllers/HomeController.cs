using CommonClassLibrary.Dto.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsServices.Model;

namespace NewsServices.Controllers;

/// <summary>
/// NewsController
/// </summary>
[ApiController]
[Route("[action]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;
    private readonly Service.NewsService _newsService;

    /// <summary>
    /// NewsController
    /// </summary>
    /// <param name="newsService"></param>
    /// <param name="logger"></param>
    public HomeController(Service.NewsService newsService, ILogger<HomeController> logger)
    {
        _newsService = newsService;
        _logger = logger;
    }

    /// <summary>
    /// Get All News For AdminPanel
    /// </summary>
    /// <param name="top"></param>
    /// <param name="skip"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>All News</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int top, int skip, CancellationToken cancellationToken)
    {
        var newses = await _newsService.GetAsync(top, skip, cancellationToken);
        return Ok(newses);
    }

    /// <summary>
    /// News With Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>News</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken)
    {
        var news = await _newsService.GetAsync(id, cancellationToken);
        return Ok(news);
    }

    /// <summary>
    /// Get News With Condition
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>All Active News</returns>
    [HttpPost]
    public async Task<IActionResult> Get([FromBody] NewsSearchDto condition, CancellationToken cancellationToken)
    {
        var newses = await _newsService.GetAsync(condition, cancellationToken);
        return Ok(newses);
    }

    /// <summary>
    /// Create News
    /// </summary>
    /// <param name="news"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>StatusCode:200</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpPut]
    public async Task<IActionResult> Create([FromBody] News news, CancellationToken cancellationToken)
    {
        await _newsService.CreateAsync(news, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Update News
    /// </summary>
    /// <param name="newsId"></param>
    /// <param name="news"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated News</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] News news, CancellationToken cancellationToken)
    {
        await _newsService.UpdateAsync(news, cancellationToken);
        var result = await _newsService.GetAsync(news.Id, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Delete News
    /// </summary>
    /// <param name="newsId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>StatusCode:200</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpDelete]
    public async Task<IActionResult> Delete(string newsId, CancellationToken cancellationToken)
    {
        await _newsService.RemoveAsync(newsId, cancellationToken);
        return Ok();
    }
}