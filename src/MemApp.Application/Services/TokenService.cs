using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using MemApp.Application.Models.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MemApp.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IMemDbContext _context;
        private readonly JWTSettings _jwtSettings;
        public TokenService(IMemDbContext context, IOptions<JWTSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }
        public RefreshTokenVm GenerateRefreshToken(string ipAddress, string appId)
        {
            return new RefreshTokenVm
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddYears(7),
                Created = DateTime.Now,
                CreatedByIp = ipAddress,
                AppId = appId

            };
        }
        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        public async Task<JwtSecurityToken> GenerateJWToken(UserProfile user, string ipAddress, string appId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(user)),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.EmailId??""),
                new Claim("uid", user.Id.ToString()),
                new Claim("user_name", user.UserName),
                new Claim("ip", ipAddress),
                new Claim("AppId", appId),
            };
            return JWTGeneration(claims);
        }

        private JwtSecurityToken JWTGeneration(IEnumerable<Claim> claims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddYears(7),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

    }
}
