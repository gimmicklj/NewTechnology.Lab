using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System;
using AuthServer.Models;
using System.Text;
using Microsoft.AspNetCore.Cors;

namespace AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")] //允许跨域
    public class AuthController : ControllerBase
    {
        private readonly IOptions<Audience> m_Settings;
        private readonly AuthContext m_DbContext;

        public AuthController(AuthContext dbContext, IOptions<Audience> settings)
        {
            m_DbContext = dbContext;
            m_Settings = settings;
        }

        [HttpPost()]
        public IActionResult Login([FromBody] User requestDTO)
        {
           
            if (!ValidateUser(requestDTO))
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = GenerateToken(requestDTO.LoginName);
            return Ok(new { access_token = token, expires_in = (int)m_Settings.Value.TokenExpiration.TotalSeconds });
        }

        private bool ValidateUser(User dbUser)
        {
            var user = m_DbContext.Users.FirstOrDefault(c => c.LoginName == dbUser.LoginName && c.Password == dbUser.Password);
            if(user == null)
            {
                return false;
            }
            return true;
        }

        private string GenerateToken(string username)
        {
            var now = DateTime.UtcNow;
            var expires = now.Add(m_Settings.Value.TokenExpiration);

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(m_Settings.Value.Secret));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: m_Settings.Value.Iss,
                audience: m_Settings.Value.Aud,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }
    }
}
