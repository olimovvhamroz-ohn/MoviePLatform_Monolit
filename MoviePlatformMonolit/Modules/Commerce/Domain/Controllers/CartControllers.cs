using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePlatformMonolit.Modules.Commerce.Application.CQRS.CartService.Command;
using MoviePlatformMonolit.Modules.Commerce.Application.CQRS.CartService.Query;
using MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Request;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Commerce.Domain.Controllers;

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