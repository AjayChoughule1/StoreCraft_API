using Microsoft.IdentityModel.Tokens;
using StoreCraft_API.Models.DTOs.AccountDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreCraft_API.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["JwtSettings:SecretKey"] ?? "StoreCraftSecretKey123456789012345678901234567890";
            _issuer = _configuration["JwtSettings:Issuer"] ?? "StoreCraftAPI";
            _audience = _configuration["JwtSettings:Audience"] ?? "StoreCraftClient";
        }

        public string GenerateToken(UserDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // Add roles as claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public UserDTO? GetUserFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);

                var userId = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var email = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var firstName = jwt.Claims.FirstOrDefault(x => x.Type == "FirstName")?.Value;
                var lastName = jwt.Claims.FirstOrDefault(x => x.Type == "LastName")?.Value;
                var roles = jwt.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                    return null;

                return new UserDTO
                {
                    Id = int.Parse(userId),
                    Email = email ?? "",
                    FirstName = firstName ?? "",
                    LastName = lastName ?? "",
                    Roles = roles
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
