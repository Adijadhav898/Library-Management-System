using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, string roleName);
    string GenerateRefreshToken();
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}