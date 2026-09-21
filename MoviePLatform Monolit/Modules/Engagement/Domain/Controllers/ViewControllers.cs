using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MoviePLatform_Monolit.Modules.Engagement.Domain.Controllers;

[ApiController]
[Route("api/views")]
[Authorize]
public class ViewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ViewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<List<ViewResponse>>>> GetViewHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var query = new GetHistoryQuery(id, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<ViewResponse>>.SuccessResponse(
            result, "View history retrieved"));
    }

    [HttpGet("continue-watching")]
    public async Task<ActionResult<ApiResponse<List<ViewResponse>>>> GetContinueWatching(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var query = new GetContinueWatchingQuery(id);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<ViewResponse>>.SuccessResponse(
            result, "Continue watching list retrieved"));
    }

    [HttpPost("record")]
    public async Task<ActionResult<ApiResponse<ViewResponse>>> RecordView(
        [FromBody] RecordViewRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var command = new RecordViewCommand(id, request.MovieId, request.PositionSeconds, request.IsCompleted);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<ViewResponse>.SuccessResponse(result, "View recorded"));
    }

    [HttpDelete("{viewId}")]
    public async Task<ActionResult<ApiResponse>> DeleteFromHistory(int viewId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        // TODO: реализовать DeleteFromHistoryCommand
        return Ok(ApiResponse.SuccessResponse("Removed from history"));
    }

    [HttpDelete("clear/all")]
    public async Task<ActionResult<ApiResponse>> ClearAllHistory()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        // TODO: реализовать ClearAllHistoryCommand
        return Ok(ApiResponse.SuccessResponse("History cleared"));
    }
}