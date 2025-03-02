namespace BuchShop.Models
{
    public class Product
    {
        public int ProductNumber { get; set; } // Primärschlüssel
        public string Productname { get; set; } = string.Empty;
        public string Productcategory { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Navigationseigenschaft
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
