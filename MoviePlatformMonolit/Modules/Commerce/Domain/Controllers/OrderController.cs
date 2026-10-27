using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePlatformMonolit.Modules.Commerce.Application.CQRS.Query;
using MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Response;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Commerce.Domain.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(IMediator mediator) : ControllerBase
{
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<OrderResponse>>>> GetUserOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (!long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        var result = await mediator.Send(new GetUserOrdersQuery(userId, pageNumber, pageSize));

        return Ok(ApiResponse<List<OrderResponse>>.SuccessResponse(result, "User orders retrieved"));
    }
}