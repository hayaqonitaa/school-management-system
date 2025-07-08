using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SchoolManagementSystem.Common.Constants;

namespace SchoolManagementSystem.Common.Helpers
{
    public class JwtHelper
    {
        public string GenerateToken(int userId, string email, int roleId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, UserRoles.GetRoleName(roleId)),
                    new Claim("RoleId", roleId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!);
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                    ValidateAudience = true,
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                
                var jwtToken = (JwtSecurityToken)validatedToken;
                return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
            }
            catch
            {
                return null;
            }
        }
    }
}
