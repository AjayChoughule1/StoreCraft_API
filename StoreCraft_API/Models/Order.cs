using System.ComponentModel.DataAnnotations;

namespace StoreCraft_API.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        // Navigation property for order items (if needed later)
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

