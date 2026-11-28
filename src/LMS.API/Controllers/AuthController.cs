using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Login and get access + refresh tokens.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);
        return result == null ? Unauthorized(new { message = "Invalid email or password" }) : Ok(result);
    }

    /// <summary>Register a new user (Admin only).</summary>
    [HttpPost("register")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(request, ct);
        return result == null ? BadRequest(new { message = "Registration failed (email may already exist)" }) : StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await _authService.RefreshTokenAsync(request, ct);
        return result == null ? Unauthorized(new { message = "Invalid or expired refresh token" }) : Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var ok = await _authService.RevokeTokenAsync(request.RefreshToken, ct);
        return ok ? Ok(new { message = "Logged out" }) : BadRequest(new { message = "Invalid token" });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();
        var ok = await _authService.ChangePasswordAsync(userId, request, ct);
        return ok ? Ok(new { message = "Password changed" }) : BadRequest(new { message = "Current password is incorrect" });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        await _authService.ForgotPasswordAsync(request, ct);
        // Always same response so we don't reveal which emails exist.
        return Ok(new { message = "If the email exists, a reset link has been sent" });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        var ok = await _authService.ResetPasswordAsync(request, ct);
        return ok ? Ok(new { message = "Password reset" }) : BadRequest(new { message = "Invalid or expired token" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me(CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();
        var user = await _authService.GetCurrentUserAsync(userId, ct);
        return user == null ? NotFound() : Ok(user);
    }
}
