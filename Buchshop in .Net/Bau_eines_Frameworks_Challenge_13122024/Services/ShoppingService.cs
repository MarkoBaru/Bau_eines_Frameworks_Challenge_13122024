using BuchShop.Models;
using BuchShop.Framework.Data;

namespace BuchShop.Services
{
    public class ShoppingService
    {
        private readonly AppDbContext _context;
        private readonly List<OrderItem> _cart; // Warenkorb

        public ShoppingService(AppDbContext context)
        {
            _context = context;
            _cart = new List<OrderItem>();
        }

        public void StartShopping(User currentUser)
        {
            while (true)
            {
                Console.WriteLine($"\nWillkommen, {currentUser.Firstname}! Einkaufsmenü:");
                Console.WriteLine("[1] Produkte durchsuchen");
                Console.WriteLine("[2] Warenkorb anzeigen");
                Console.WriteLine("[3] Kauf abschliessen");
                Console.WriteLine("[4] Zurück zum Hauptmenü");

                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        BrowseProducts();
                        break;
                    case "2":
                        ShowCart();
                        break;
                    case "3":
                        Checkout(currentUser);
                        return;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Ungültige Eingabe.");
                        break;
                }
            }
        }

        private void BrowseProducts()
        {
            Console.Write("\nProduktname eingeben (oder leer lassen, um alle Produkte zu sehen): ");
            string searchName = Console.ReadLine() ?? string.Empty;

            var products = string.IsNullOrWhiteSpace(searchName)
                ? _context.Products.ToList()
                : _context.Products.Where(p => p.Productname.Contains(searchName)).ToList();

            if (products.Any())
            {
                foreach (var product in products)
                {
                    Console.WriteLine($"ID: {product.ProductNumber}, Name: {product.Productname}, Kategorie: {product.Productcategory}, Preis: {product.Price:C}, Bestand: {product.Stock}");
                }

                Console.Write("\nMöchtest du ein Produkt zum Warenkorb hinzufügen? (Ja/Nein): ");
                if (Console.ReadLine()?.Trim().ToLower() == "ja")
                {
                    AddToCart();
                }
            }
            else
            {
                Console.WriteLine("Keine Produkte gefunden.");
            }
        }

        private void AddToCart()
        {
            Console.Write("\nProduktnummer eingeben: ");
            if (!int.TryParse(Console.ReadLine(), out int productNumber))
            {
                Console.WriteLine("Ungültige Produktnummer.");
                return;
            }

            var product = _context.Products.FirstOrDefault(p => p.ProductNumber == productNumber);

            if (product == null || product.Stock <= 0)
            {
                Console.WriteLine("Produkt nicht verfügbar.");
                return;
            }

            Console.Write("Menge eingeben: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0 || quantity > product.Stock)
            {
                Console.WriteLine("Ungültige Menge.");
                return;
            }

            _cart.Add(new OrderItem
            {
                ProductNumber = product.ProductNumber,
                Product = product,
                Quantity = quantity
            });

            product.Stock -= quantity;
            _context.SaveChanges();

            Console.WriteLine("Produkt erfolgreich zum Warenkorb hinzugefügt.");
        }

        private void ShowCart()
        {
            if (!_cart.Any())
            {
                Console.WriteLine("Der Warenkorb ist leer.");
                return;
            }

            Console.WriteLine("\nWarenkorb:");
            foreach (var item in _cart)
            {
                Console.WriteLine($"Produkt: {item.Product.Productname}, Menge: {item.Quantity}, Einzelpreis: {item.Product.Price:C}, Gesamtpreis: {item.Quantity * item.Product.Price:C}");
            }
        }

        private void Checkout(User currentUser)
        {
            if (!_cart.Any())
            {
                Console.WriteLine("Der Warenkorb ist leer. Es gibt nichts zu kaufen.");
                return;
            }

            var total = _cart.Sum(item => item.Quantity * item.Product.Price);
            Console.WriteLine($"\nGesamtsumme: CHF {total:C}");

            Console.Write("Möchtest du den Kauf abschliessen? (Ja/Nein): ");
            if (Console.ReadLine()?.Trim().ToLower() == "ja")
            {
                var order = new CustomerOrder
                {
                    UserEmail = currentUser.UserEmail, // Verknüpfung mit dem aktuellen Benutzer
                    OrderItems = _cart.ToList(),
                    Date = DateTime.Now,
                    TotalAmount = total
                };

                _context.CustomerOrders.Add(order);
                _context.SaveChanges();

                _cart.Clear();

                Console.WriteLine("Kauf erfolgreich abgeschlossen. Vielen Dank!");
            }
            else
            {
                Console.WriteLine("Kauf abgebrochen.");
            }
        }
    }
}
