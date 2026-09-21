// Modules/Commerce/Domain/Contoller/CartControllers.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CartController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{userId}")]
    public async Task<ActionResult<CartResponse>> GetCart(int userId)
        => Ok(await _mediator.Send(new GetActiveCartByUserId(userId)));

    [HttpPost("{userId}/items/{movieId}")]
    public async Task<ActionResult<CartResponse>> AddMovie(int userId, int movieId)
        => Ok(await _mediator.Send(new AddMovieToCart(userId, movieId)));

    [HttpDelete("{userId}/items/{movieId}")]
    public async Task<ActionResult<CartResponse>> RemoveMovie(int userId, int movieId)
        => Ok(await _mediator.Send(new RemoveMovieFromCartCommand(userId, movieId)));

    [HttpPost("{userId}/checkout")]
    public async Task<ActionResult<CartResponse>> Checkout(int userId)
        => Ok(await _mediator.Send(new ChekoutCartCommand(userId)));
}