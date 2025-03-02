using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BuchShop.Framework.Data;
using BuchShop.Models;
using System.Collections.Generic;

namespace BuchShop.Services
{
    /// <summary>
    /// Kümmert sich um Datenbankzugriffe: Produkte hinzufügen, suchen, aktualisieren etc.
    /// </summary>
    public class InventoryDataService
    {
        private readonly AppDbContext _context;

        public InventoryDataService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hinzufügen eines neuen Produkts (DB generiert automatisch ProductNumber).
        /// </summary>
        public void AddProduct()
        {
            Console.Write("\nProduktname: ");
            string name = Console.ReadLine() ?? string.Empty;

            Console.Write("Kategorie: ");
            string category = Console.ReadLine() ?? string.Empty;

            Console.Write("Preis: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Ungültiger Preis.");
                return;
            }

            Console.Write("Bestand: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("Ungültiger Bestand.");
                return;
            }

            var product = new Product
            {
                Productname = name,
                Productcategory = category,
                Price = price,
                Stock = stock
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            Console.WriteLine("Produkt erfolgreich hinzugefügt.");
        }

        /// <summary>
        /// Sucht nach Produkten anhand eines eingegebenen Namens (oder zeigt alle an).
        /// </summary>
        public void SearchProduct()
        {
            Console.Write("\nProduktname eingeben (oder leer lassen, um alle Produkte zu sehen): ");
            string searchName = Console.ReadLine() ?? string.Empty;

            var products = string.IsNullOrWhiteSpace(searchName)
                ? _context.Products.ToList()
                : _context.Products.Where(p => EF.Functions.Like(p.Productname, $"%{searchName}%")).ToList();

            if (products.Any())
            {
                foreach (var product in products)
                {
                    Console.WriteLine($"ID: {product.ProductNumber}, Name: {product.Productname}, Kategorie: {product.Productcategory}, Preis: {product.Price:C}, Bestand: {product.Stock}");
                }
            }
            else
            {
                Console.WriteLine("Kein Produkt gefunden.");
            }
        }

        /// <summary>
        /// Aktualisiert den Produktbestand eines ausgewählten Produkts anhand der ID.
        /// </summary>
        public void UpdateProductStock()
        {
            Console.Write("\nProduktnummer eingeben: ");
            if (!int.TryParse(Console.ReadLine(), out int productNumber))
            {
                Console.WriteLine("Ungültige Produktnummer.");
                return;
            }

            var product = _context.Products.FirstOrDefault(p => p.ProductNumber == productNumber);

            if (product != null)
            {
                Console.Write($"Aktueller Bestand: {product.Stock}. Neuer Bestand: ");
                if (!int.TryParse(Console.ReadLine(), out int newStock))
                {
                    Console.WriteLine("Ungültiger Bestand.");
                    return;
                }

                product.Stock = newStock;
                _context.SaveChanges();

                Console.WriteLine("Produktbestand erfolgreich aktualisiert.");
            }
            else
            {
                Console.WriteLine("Produkt nicht gefunden.");
            }
        }

        /// <summary>
        /// Gibt alle Produkte aus der Datenbank zurück.
        /// </summary>
        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        /// <summary>
        /// Aktualisiert oder fügt ein Produkt anhand seines Namens hinzu.
        /// (Verwendet beim Import, um doppelte Einträge zu vermeiden.)
        /// </summary>
        public void AddOrUpdateByName(Product product)
        {
            var existingProduct = _context.Products
                .FirstOrDefault(p => p.Productname == product.Productname);

            if (existingProduct == null)
            {
                // Produkt existiert nicht -> neu hinzufügen
                _context.Products.Add(product);
            }
            else
            {
                // Produkt existiert -> aktualisieren
                existingProduct.Productcategory = product.Productcategory;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
            }
        }

        /// <summary>
        /// Speichert Änderungen in der DB (z.B. nach einem Batch-Import).
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
