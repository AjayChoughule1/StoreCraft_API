using System.ComponentModel.DataAnnotations;

namespace StoreCraft_API.Models.DTOs.AccountDTOs
{
    public class AssignRoleDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
