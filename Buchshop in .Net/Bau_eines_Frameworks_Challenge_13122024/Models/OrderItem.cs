namespace BuchShop.Models
{
    public class OrderItem
    {
        public int Id { get; set; } // Primärschlüssel
        public int ProductNumber { get; set; } // Verweis auf Produkt
        public int CustomerOrderId { get; set; } // Verweis auf Bestellung
        public int Quantity { get; set; } // Bestellte Menge

        // Navigationseigenschaften
        public virtual Product Product { get; set; } = null!;
        public virtual CustomerOrder CustomerOrder { get; set; } = null!;
    }
}
