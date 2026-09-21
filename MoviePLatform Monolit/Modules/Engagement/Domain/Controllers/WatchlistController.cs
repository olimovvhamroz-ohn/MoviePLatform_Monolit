using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MoviePLatform_Monolit.Modules.Engagement.Domain.Controllers;

[ApiController]
[Route("api/watchlist")]
[Authorize]
public class WatchlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WatchlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<WatchlistResponse>>>> GetWatchlist(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var query = new GetWatchlistByUserIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<WatchlistResponse>>.SuccessResponse(
            result, "Watchlist retrieved"));
    }

    [HttpGet("check/{movieId}")]
    public async Task<ActionResult<ApiResponse<bool>>> IsInWatchlist(int movieId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var query = new IsInWatchlistQuery(id, movieId);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<bool>.SuccessResponse(
            result, result ? "Movie is in watchlist" : "Movie is not in watchlist"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<WatchlistResponse>>> AddToWatchlist(
        [FromBody] WatchlistRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var command = new CreateWatchlistCommand(id, request.MovieId);
        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetWatchlist), null,
            ApiResponse<WatchlistResponse>.SuccessResponse(result, "Added to watchlist", 201));
    }

    [HttpDelete("{movieId}")]
    public async Task<ActionResult<ApiResponse>> RemoveFromWatchlist(int movieId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var command = new DeleteWatchlistCommand(id, movieId);
        await _mediator.Send(command);
        return Ok(ApiResponse.SuccessResponse("Removed from watchlist"));
    }

    [HttpPost("mark-watched/{movieId}")]
    public async Task<ActionResult<ApiResponse<WatchlistResponse>>> MarkAsWatched(int movieId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        var command = new MarkWatchedCommand(id, movieId);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<WatchlistResponse>.SuccessResponse(result, "Marked as watched"));
    }

    [HttpGet("count")]
    public async Task<ActionResult<ApiResponse<int>>> GetWatchlistCount()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(userId, out var id))
            return Unauthorized();

        // TODO: реализовать GetWatchlistCountQuery
        return Ok(ApiResponse<int>.SuccessResponse(0, "Watchlist count retrieved"));
    }
}