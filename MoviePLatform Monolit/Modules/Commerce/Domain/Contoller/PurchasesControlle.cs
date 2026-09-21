using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Domain.Controllers;

[ApiController]
[Route("api/purchases")]
[Authorize]
public class PurchasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PurchaseResponse>>>> GetUserPurchases(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized();

        var query = new GetUserPurchasesQuery(id);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<PurchaseResponse>>.SuccessResponse(
            result, "User purchases retrieved"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PurchaseResponse>>> CreatePurchase(
        [FromBody] PurchaseRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized();

        var command = new CreatePurchaseCommand(new PurchaseRequest { UserId = id, MovieId = request.MovieId });
        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetUserPurchases), null,
            ApiResponse<PurchaseResponse>.SuccessResponse(result, "Purchase created", 201));
    }

    [HttpGet("check/{movieId}")]
    public async Task<ActionResult<ApiResponse<bool>>> HasUserPurchasedMovie(int movieId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized();

        var query = new HasUserPurchasedMovieQuery(id, movieId);
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<bool>.SuccessResponse(
            result, result ? "User has purchased this movie" : "User has not purchased this movie"));
    }

    [HttpGet("{purchaseId}")]
    public async Task<ActionResult<ApiResponse<PurchaseResponse>>> GetPurchaseById(int purchaseId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized();

        // TODO: реализовать GetPurchaseByIdQuery с проверкой владельца
        return Ok(ApiResponse<PurchaseResponse>.SuccessResponse(
            new PurchaseResponse(), "Purchase retrieved"));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/all")]
    public async Task<ActionResult<ApiResponse<List<PurchaseResponse>>>> GetAllPurchases(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // TODO: реализовать GetAllPurchasesQuery
        return Ok(ApiResponse<List<PurchaseResponse>>.SuccessResponse(
            new List<PurchaseResponse>(), "All purchases retrieved"));
    }
}