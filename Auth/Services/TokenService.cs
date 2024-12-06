using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Models;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateToken(User user)
        {
            // Add the role claim to the token
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, "admin") // Add role claim with value "admin"
    };

            // Ensure the secret key is secure and of sufficient length
            if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey) || _jwtSettings.SecretKey.Length < 32)
                throw new ArgumentException("Secret key must be at least 32 characters long.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)); // Ensure UTF8 encoding
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var principal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true // Ensure the token has not expired
                }, out _);

                // Check if the role claim is "admin"
                var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                return roleClaim == "admin";
            }
            catch
            {
                return false;
            }
        }
    }
}
