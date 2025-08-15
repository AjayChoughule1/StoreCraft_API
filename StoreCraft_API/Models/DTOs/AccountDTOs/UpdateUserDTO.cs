using System.ComponentModel.DataAnnotations;

namespace StoreCraft_API.Models.DTOs.AccountDTOs
{
    public class UpdateUserDTO
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
