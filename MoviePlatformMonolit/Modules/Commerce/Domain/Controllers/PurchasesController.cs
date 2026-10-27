using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePlatformMonolit.Modules.Commerce.Application.CQRS.PurchaseService.Command;
using MoviePlatformMonolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;
using MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Request;
using MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Response;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Commerce.Domain.Controllers;

[ApiController]
[Route("api/purchases")]
[Authorize]
public class PurchasesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PurchaseResponse>>>> GetUserPurchases(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized();

        var query = new GetUserPurchasesQuery(id);
        var result = await mediator.Send(query);
        return Ok(ApiResponse<List<PurchaseResponse>>.SuccessResponse(
            result, "User purchases retrieved"));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PurchaseResponse>>> CreatePurchase(
        [FromBody] PurchaseRequest request)
    {
        var command = new CreatePurchaseCommand(
            new PurchaseRequest {
             UserId = request.UserId,
             MovieId = request.MovieId });
        
        var result = await mediator.Send(command);

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
        var result = await mediator.Send(query);
        return Ok(ApiResponse<bool>.SuccessResponse(
            result, result ? "User has purchased this movie" : "User has not purchased this movie"));
    }
    [HttpGet("get-by-ID/{purchaseId}")]
    public async Task<ActionResult<ApiResponse<PurchaseResponse>>> GetUserPurchasedById(long purchaseId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(userId, out var id))
        {
            return Unauthorized();
        }

        var query = new GetPurchaseByIdQuery(id, purchaseId);
        var result = await mediator.Send(query);

        return Ok(ApiResponse<PurchaseResponse>.SuccessResponse(
            result, "Purchase retrieved"));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/all")]
    public async Task<ActionResult<ApiResponse<List<PurchaseResponse>>>> GetAllPurchases(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new  GetAllPurchasesQuery(pageNumber,pageSize); 
        await mediator.Send(query);
        return Ok(ApiResponse<List<PurchaseResponse>>.SuccessResponse(
            new List<PurchaseResponse>(), "All purchases retrieved"));
    }
}