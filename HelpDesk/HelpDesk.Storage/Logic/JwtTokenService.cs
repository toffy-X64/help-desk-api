using HelpDesk.Core.Abstractions;
using HelpDesk.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Storage.Logic
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        public JwtTokenService(IConfiguration configuration)
        {
            _secret = configuration["JWT:Secret"] ??
                throw new Exception("`JWT:Secret` not found in configuration");

            _issuer = configuration["JWT:Issuer"] ??
                throw new Exception("`JWT:Issuer` not found in configuration");

            _audience = configuration["JWT:Audience"] ??
                throw new Exception("`JWT:Audience` not found in configuration");
        }

        public string GenerateUserToken(User user)
        {
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _issuer,
                audience: _audience,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
