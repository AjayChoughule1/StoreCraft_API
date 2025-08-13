using System.ComponentModel.DataAnnotations;

namespace StoreCraft_API.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
