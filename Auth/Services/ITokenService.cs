using Auth.Models;

namespace Auth.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }
}
