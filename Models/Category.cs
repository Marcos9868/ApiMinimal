using System.Text.Json.Serialization;

namespace ApiMinimal.Models
{
    public class Category
    {
        [JsonIgnore]
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<Product>? Products { get; set; }
    }
}