using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.CartService.Command;
using MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.CartService.Query;
using MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.Request;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;
namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController(IMediator mediator) : ControllerBase
{
    private bool TryGetUserId(out long userId)
        => long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier) ?.Value, out userId)&& userId > 0;

    [HttpGet]
    public async Task<ActionResult<CartResponse>> GetCart()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        
        return Ok(await mediator.Send(new GetActiveCartByUserId(userId)));
    }

    [HttpPost("items/{movieId}")]
    public async Task<ActionResult<CartResponse>> AddMovieToCart(int movieId)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        
        return Ok(await mediator.Send(new AddMovieToCart(userId, movieId)));
    }

    [HttpDelete("items/{movieId}")]
    public async Task<ActionResult<CartResponse>> RemoveMovie(int movieId)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        
        return Ok(await mediator.Send(new RemoveMovieFromCartCommand(userId, movieId)));
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<CartResponse>> Checkout([FromBody] CheckoutRequest? request = null)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        
        return Ok(await mediator.Send(new CheckoutCartCommand(userId, request?.PaymentToken)));
    }
}