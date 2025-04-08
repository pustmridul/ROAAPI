using MemApp.Application.Models;
using MemApp.Application.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MemApp.Application.Interfaces
{
    public interface ITokenService
    {
        RefreshTokenVm GenerateRefreshToken(string ipAddress, string appId);
        Task<JwtSecurityToken> GenerateJWToken(UserProfile user, string ipAddress, string appId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    }
}
