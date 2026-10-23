using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Views.Command;
using MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Views.Query;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.REQUEST;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Domain.Controllers;

[ApiController]
[Route("api/views")]
[Authorize]
public class ViewsController(IMediator mediator) : ControllerBase
{
    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<List<ViewResponse>>>> GetViewHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var query = new GetHistoryQuery(id, pageNumber, pageSize);
        var result = await mediator.Send(query);
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
        var result = await mediator.Send(query);
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
        var result = await mediator.Send(command);
        return Ok(ApiResponse<ViewResponse>.SuccessResponse(result, "View recorded"));
    }

    [HttpDelete("{viewId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteFromHistory(long viewId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();
        
        var command=new DeleteFromHistoryCommand(id, viewId);
        await mediator.Send(command);

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    [HttpDelete("clear/all")]
    public async Task<ActionResult<ApiResponse<bool>>> ClearAllHistory()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();
        var command = new ClearAllHistoryCommand(id);
        await mediator.Send(command);
        
        return Ok(ApiResponse.SuccessResponse("History cleared"));
    }
}