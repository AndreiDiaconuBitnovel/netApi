using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication2.Utils
{
    public static class JwtToken
    {
        public static string CreateToken(string userName, IConfiguration config)
        {

            List<Claim> claims = new List<Claim>()
            {
                new Claim("userName", userName ),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                config.GetSection("JwtConfig:Secret").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
