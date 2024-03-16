using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMinimal.Models
{
    public class Product
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0_0;
        public string Image { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public int Storage { get; set; } = 0;
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; } = 0;
        public Category? Category { get; set; }
    }
}