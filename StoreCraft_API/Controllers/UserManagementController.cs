using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreCraft_API.Helpers;
using StoreCraft_API.Models.DTOs.AccountDTOs;
using StoreCraft_API.Services;
using System.Reflection;

namespace StoreCraft_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IAuthService _authService;
        public UserManagementController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<List<UserDTO>>>> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();

                return Ok(ApiResponse<List<UserDTO>>.SuccessResponse(users, "Users retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<UserDTO>>.ErrorResponse("An error occurred while retrieving users"));
            }
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<ApiResponse<UserDTO>>> GetUser(int id)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(ApiResponse<UserDTO>.ErrorResponse("User not found"));
                }

                return Ok(ApiResponse<UserDTO>.SuccessResponse(user, "User retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDTO>.ErrorResponse("An error occurred while retrieving user"));
            }
        }

        [HttpPost("assign-role")]
        public async Task<ActionResult<ApiResponse<bool>>> AssignRole(AssignRoleDTO assignRoleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Validation failed", errors));
                }

                var result = await _authService.AssignRoleToUserAsync(assignRoleDto.UserId, assignRoleDto.RoleId);

                if (!result)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to assign role. Role may already be assigned or user/role doesn't exist."));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Role assigned successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while assigning role"));
            }
        }

        [HttpPost("remove-role")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveRole(AssignRoleDTO removeRoleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Validation failed", errors));
                }

                var result = await _authService.RemoveRoleFromUserAsync(removeRoleDto.UserId, removeRoleDto.RoleId);

                if (!result)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to remove role. Role may not be assigned or user/role doesn't exist."));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Role removed successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while removing role"));
            }
        }

        [HttpPut("deactivate/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeactivateUser(int id)
        {
            try
            {
                var result = await _authService.DeactivateUserAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "User deactivated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deactivating user"));
            }
        }

        [HttpPut("activate/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> ActivateUser(int id)
        {
            try
            {
                var result = await _authService.ActivateUserAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "User activated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while activating user"));
            }
        }
    }
}
