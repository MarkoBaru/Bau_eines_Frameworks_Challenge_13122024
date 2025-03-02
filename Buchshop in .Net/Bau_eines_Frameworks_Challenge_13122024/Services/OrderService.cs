using BuchShop.Models;
using BuchShop.Framework.Data;


namespace BuchShop.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;
        private readonly User _user;

        public OrderService(AppDbContext context, User user)
        {
            _context = context;
            _user = user;
        }

        public void DisplayShopMenu()
        {
            Console.WriteLine("\nProdukte:");
            var products = _context.Products.ToList();

            if (products.Count == 0)
            {
                Console.WriteLine("Keine Produkte verfügbar.");
                return;
            }

            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.ProductNumber}, Name: {product.Productname}, Preis: {product.Price}, Bestand: {product.Stock}");
            }

            Console.Write("\nProduktnummer wählen: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Ungültige Eingabe.");
                return;
            }

            Console.Write("Menge eingeben: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity))
            {
                Console.WriteLine("Ungültige Eingabe.");
                return;
            }

            var productToBuy = _context.Products.FirstOrDefault(p => p.ProductNumber == productId);

            if (productToBuy != null && productToBuy.Stock >= quantity)
            {
                productToBuy.Stock -= quantity;

                var orderItem = new OrderItem
                {
                    ProductNumber = productToBuy.ProductNumber,
                    Quantity = quantity
                };

                var order = new CustomerOrder
                {
                    Date = DateTime.Now,
                    UserEmail = _user.UserEmail,
                    OrderItems = new List<OrderItem> { orderItem }
                };

                _context.CustomerOrders.Add(order);
                _context.SaveChanges();

                Console.WriteLine("Produkt erfolgreich gekauft!");
            }
            else if (productToBuy != null && productToBuy.Stock < quantity)
            {
                Console.WriteLine("Nicht genügend Bestand für die gewünschte Menge.");
            }
            else
            {
                Console.WriteLine("Ungültige Produktnummer.");
            }
        }
    }
}
