using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // История заказов текущего пользователя (userId берётся только из токена)
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<OrderResponse>>>> GetUserOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (!long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetUserOrdersQuery(userId, pageNumber, pageSize));

        return Ok(ApiResponse<List<OrderResponse>>.SuccessResponse(result, "User orders retrieved"));
    }
}