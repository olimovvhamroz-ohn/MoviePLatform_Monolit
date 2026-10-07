using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Domain.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthController( IAuthService authService, IUserRepository userRepository, IJwtService jwtService)
    {
        _authService = authService;
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterUserRequest request)
    {
        var result = await _authService.Register(request);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "User registered successfully", 201));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest request)
    {
        var loginResult = await _authService.Login(request.Email, request.Password);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(loginResult, "Login successful"));
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(userId, out var id))
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse(
                "Invalid token", statusCode: 401));

        var user = await _userRepository.GetByIdAsync(id);
        var newToken = _jwtService.GenerateToken(user);

        var response = new LoginResponse
        {
            Token = newToken
        };

        return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Token refreshed"));
    }

    [Authorize]
    [HttpPost("logout")]
    public ActionResult<ApiResponse> Logout()
    {
        return Ok(ApiResponse.SuccessResponse("Logged out successfully"));
    }

    [Authorize]
    [HttpPost("verify-token")]
    public ActionResult<ApiResponse<bool>> VerifyToken()
    {
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Token is valid"));
    }
}