using System.ComponentModel.DataAnnotations;

namespace StoreCraft_API.Models
{
    public class ProductLog
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(255)]
        public string Thread { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Level { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Logger { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Exception { get; set; }
    }
}
