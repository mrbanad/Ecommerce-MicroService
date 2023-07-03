using CommentServices.Model;
using CommonClassLibrary.Dto.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommentServices.Controllers;

/// <summary>
/// CommentController
/// </summary>
[ApiController]
[Route("[action]")]
public class HomeController : ControllerBase
{
    private readonly Service.CommentService _commentService;
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// CommentController
    /// </summary>
    /// <param name="commentService"></param>
    /// <param name="logger"></param>
    public HomeController(Service.CommentService commentService, ILogger<HomeController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// Get All Comment For AdminPanel
    /// </summary>
    /// <param name="top"></param>
    /// <param name="skip"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>All Comment</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int top, int skip, CancellationToken cancellationToken)
    {
        var comments = await _commentService.GetAsync(top, skip, cancellationToken);
        return Ok(comments);
    }

    /// <summary>
    /// Comment With Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Comment</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken)
    {
        var comment = await _commentService.GetAsync(id, cancellationToken);
        return Ok(comment);
    }

    /// <summary>
    /// Get Comment With Condition
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>All Active Comment</returns>
    [HttpPost]
    public async Task<IActionResult> Get([FromBody] CommentSearchDto condition, CancellationToken cancellationToken)
    {
        var comments = await _commentService.GetAsync(condition, cancellationToken);
        return Ok(comments);
    }

    /// <summary>
    /// Create Comment
    /// </summary>
    /// <param name="comment"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>StatusCode:200</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpPut]
    public async Task<IActionResult> Create([FromBody] Comment comment, CancellationToken cancellationToken)
    {
        await _commentService.CreateAsync(comment, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Update Comment
    /// </summary>
    /// <param name="commentId"></param>
    /// <param name="comment"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated Comment</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] Comment comment, CancellationToken cancellationToken)
    {
        await _commentService.UpdateAsync(comment, cancellationToken);
        var result = await _commentService.GetAsync(comment.Id, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Delete Comment
    /// </summary>
    /// <param name="commentId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>StatusCode:200</returns>
    /// <remarks>This method requires authentication.</remarks>
    [Authorize("Administrator")]
    [HttpDelete]
    public async Task<IActionResult> Delete(string commentId, CancellationToken cancellationToken)
    {
        await _commentService.RemoveAsync(commentId, cancellationToken);
        return Ok();
    }
}