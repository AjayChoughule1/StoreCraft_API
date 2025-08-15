using StoreCraft_API.Models.DTOs.AccountDTOs;

namespace StoreCraft_API.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserDTO user);
        bool ValidateToken(string token);
        UserDTO? GetUserFromToken(string token);
    }
}
