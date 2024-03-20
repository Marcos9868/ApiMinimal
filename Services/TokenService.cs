using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiMinimal.Models;
using Microsoft.IdentityModel.Tokens;

namespace ApiMinimal.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateToken(string Key, string Issuer, string Audience, User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };

            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(Issuer, Audience, claims, expires: DateTime.Now.AddHours(3), signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
    }
}