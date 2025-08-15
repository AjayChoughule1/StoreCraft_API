using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreCraft_API.Helpers;
using StoreCraft_API.Models.DTOs.AccountDTOs;
using StoreCraft_API.Services;
using System.Reflection;
using System.Security.Claims;

namespace StoreCraft_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Register(RegisterDTO registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<AuthResponseDTO>.ErrorResponse("Validation failed", errors));
                }

                var result = await _authService.RegisterAsync(registerDto);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<AuthResponseDTO>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "User registered successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponseDTO>.ErrorResponse("An error occurred during registration"));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDTO>>> Login(LoginDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<AuthResponseDTO>.ErrorResponse("Validation failed", errors));
                }

                var result = await _authService.LoginAsync(loginDto);

                if (!result.Success)
                {
                    return Unauthorized(ApiResponse<AuthResponseDTO>.ErrorResponse(result.Message));
                }

                return Ok(ApiResponse<AuthResponseDTO>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponseDTO>.ErrorResponse("An error occurred during login"));
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(ChangePasswordDTO changePasswordDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Validation failed", errors));
                }

                var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

                if (!result)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to change password. Please check your current password."));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Password changed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while changing password"));
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDTO>>> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(ApiResponse<UserDTO>.ErrorResponse("User not found"));
                }

                return Ok(ApiResponse<UserDTO>.SuccessResponse(user, "Profile retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDTO>.ErrorResponse("An error occurred while retrieving profile"));
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDTO>>> UpdateProfile(UpdateUserDTO updateUserDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<UserDTO>.ErrorResponse("Validation failed", errors));
                }

                var result = await _authService.UpdateUserAsync(userId, updateUserDto);

                return Ok(ApiResponse<UserDTO>.SuccessResponse(result, "Profile updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse<UserDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDTO>.ErrorResponse("An error occurred while updating profile"));
            }
        }

        // Helper method to get current user ID from JWT token
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
