using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Users.Application.CQRS.AuthService;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Users.Domain.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterUserRequest request)
    {
        var result = await authService.Register(request);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<LoginResponse>.SuccessResponse(result, "User registered successfully"));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await authService.Login(request.Email, request.Password);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Login successful"));
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken()
    {
        if (!long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId) || userId <= 0)
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Invalid token", statusCode: 401));

        var result = await authService.RefreshToken(userId);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Token refreshed"));
    }

    [Authorize]
    [HttpPost("verify-token")]
    public ActionResult<ApiResponse<bool>> VerifyToken()
    {
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Token is valid"));
    }
}