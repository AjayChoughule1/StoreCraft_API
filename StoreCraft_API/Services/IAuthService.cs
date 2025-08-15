using StoreCraft_API.Models.DTOs.AccountDTOs;

namespace StoreCraft_API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDto);
        Task<UserDTO?> GetUserByIdAsync(int userId);
        Task<UserDTO?> GetUserByEmailAsync(string email);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<bool> DeactivateUserAsync(int userId);
        Task<bool> ActivateUserAsync(int userId);
        Task<UserDTO> UpdateUserAsync(int userId, UpdateUserDTO updateUserDto);
    }
}
