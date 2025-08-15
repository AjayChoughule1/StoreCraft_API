using log4net;
using StoreCraft_API.Data;
using StoreCraft_API.Models.DTOs.AccountDTOs;
using StoreCraft_API.Models;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace StoreCraft_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        public AuthService(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
                if (existingUser != null)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "User with this email already exists"
                    };
                }

                // Create new user
                var user = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    PasswordHash = HashPassword(registerDto.Password),
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Assign default role (Customer)
                var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
                if (customerRole != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = customerRole.Id,
                        AssignedDate = DateTime.Now
                    };
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }

                // Generate JWT token
                var userDto = await GetUserDtoAsync(user);
                var token = _jwtService.GenerateToken(userDto);
                var expiresAt = DateTime.UtcNow.AddHours(24);


                return new AuthResponseDTO
                {
                    Success = true,
                    Message = "User registered successfully",
                    Token = token,
                    ExpiresAt = expiresAt,
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                if (!user.IsActive)
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "Your account has been deactivated"
                    };
                }

                if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return new AuthResponseDTO
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Update last login date
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();

                // Generate JWT token
                var userDto = await GetUserDtoAsync(user);
                var token = _jwtService.GenerateToken(userDto);
                var expiresAt = DateTime.UtcNow.AddHours(24);

                return new AuthResponseDTO
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    ExpiresAt = expiresAt,
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDTO
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                return user != null ? await GetUserDtoAsync(user) : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Email == email);

                return user != null ? await GetUserDtoAsync(user) : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            try
            {
                var existingUserRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existingUserRole != null)
                {
                    return false;
                }

                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedDate = DateTime.Now
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (userRole == null)
                {
                    return false;
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .ToListAsync();

                var userDtos = new List<UserDTO>();
                foreach (var user in users)
                {
                    userDtos.Add(await GetUserDtoAsync(user));
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                return new List<UserDTO>();
            }
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.IsActive = false;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ActivateUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.IsActive = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<UserDTO> UpdateUserAsync(int userId, UpdateUserDTO updateUserDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    throw new ArgumentException($"User with ID {userId} not found");

                user.FirstName = updateUserDto.FirstName;
                user.LastName = updateUserDto.LastName;
                user.PhoneNumber = updateUserDto.PhoneNumber;

                await _context.SaveChangesAsync();

                return await GetUserDtoAsync(user);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Private helper methods
        private async Task<UserDTO> GetUserDtoAsync(User user)
        {
            var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                Roles = roles
            };
        }

        //private string HashPassword(string password)
        //{
        //    using var sha256 = SHA256.Create();
        //    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "StoreCraftSalt123"));
        //    return Convert.ToBase64String(hashedBytes);
        //}

        //private bool VerifyPassword(string password, string hashPassword)
        //{
        //    var hashedInputPassword = HashPassword(password);
        //    return hashedInputPassword == hashPassword;
        //}
        //
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashPassword)
        {
            var hashedInputPassword = HashPassword(password);
            return hashedInputPassword == hashPassword;
        }

    }
}
