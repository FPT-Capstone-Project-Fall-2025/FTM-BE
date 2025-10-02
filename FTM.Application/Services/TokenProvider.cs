using FTM.Application.IServices;
using FTM.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly JwtOptions _jwtOptions;

        public TokenProvider()
        {
            _jwtOptions = new JwtOptions()
            {
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                ExpireDays = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_DAYS") ?? "7"),
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                SigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY")
            };
        }

        public string GetIssuer()
        {
            return Environment.GetEnvironmentVariable("JWT_ISSUER");
        }

        public string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtOptions.ExpireDays));
            var token = CreateJwtToken(claims, expires);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateThirdPartyJwtToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var token = CreateJwtToken(claims, expires);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken CreateJwtToken(IEnumerable<Claim> claims, DateTime? _expires)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: _expires,
                signingCredentials: creds
            );
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)),
                ValidateLifetime = false
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
