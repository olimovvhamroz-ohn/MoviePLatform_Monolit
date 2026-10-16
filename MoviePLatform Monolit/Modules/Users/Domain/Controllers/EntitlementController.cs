using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviePLatform_Monolit.Modules.Users.Application.CQRS.EntitlementService.Command;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Domain.Controllers;

[ApiController]
[Route("api/entitlement")]
[Authorize]
public class EntitlementController(IMediator mediator) : ControllerBase
{
    private bool TryGetUserId(out long id) =>
        long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out id) && id > 0;

    [HttpGet("{movieId:long}")]
    public async Task<ActionResult<MovieAccessResultResponse>> Check(long movieId)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await mediator.Send(new CheckEntitlementQuery(userId, movieId)));
    }

    [HttpPost("{movieId:long}/start")]
    public async Task<ActionResult<MovieAccessResultResponse>> Start(long movieId)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await mediator.Send(new StartPlaybackCommand(userId, movieId)));
    }
}