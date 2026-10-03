using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Contoller;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CartController(IMediator mediator) => _mediator = mediator;

    // userId танҳо аз токен гирифта мешавад
    private bool TryGetUserId(out int userId)
        => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId) && userId > 0;

    [HttpGet]
    public async Task<ActionResult<CartResponse>> GetCart()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _mediator.Send(new GetActiveCartByUserId(userId)));
    }

    [HttpPost("items/{movieId}")]
    public async Task<ActionResult<CartResponse>> AddMovie(int movieId)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _mediator.Send(new AddMovieToCart(userId, movieId)));
    }

    [HttpDelete("items/{movieId}")]
    public async Task<ActionResult<CartResponse>> RemoveMovie(int movieId)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _mediator.Send(new RemoveMovieFromCartCommand(userId, movieId)));
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<CartResponse>> Checkout([FromBody] CheckoutRequest? request = null)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _mediator.Send(new ChekoutCartCommand(userId, request?.PaymentToken)));
    }
}