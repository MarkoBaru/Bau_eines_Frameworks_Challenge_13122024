using BuchShop.Models;
using BuchShop.Framework.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuchShop.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verwaltung der Inventaroptionen inklusive Import/Export.
        /// </summary>
        public void ManageInventory()
        {
            while (true)
            {
                Console.WriteLine("\nLagerverwaltung:");
                Console.WriteLine("[1] Neues Produkt hinzufügen");
                Console.WriteLine("[2] Produkt suchen");
                Console.WriteLine("[3] Produktbestand aktualisieren");
                Console.WriteLine("[4] Produkte exportieren");
                Console.WriteLine("[5] Produkte importieren");
                Console.WriteLine("[6] Zurück zum Hauptmenü");

                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1":
                        AddProduct();
                        break;
                    case "2":
                        SearchProduct();
                        break;
                    case "3":
                        UpdateProductStock();
                        break;
                    case "4":
                        ExportProducts();
                        break;
                    case "5":
                        ImportProducts();
                        break;
                    case "6":
                        Console.WriteLine("Zurück zum Hauptmenü...");
                        return;
                    default:
                        Console.WriteLine("Ungültige Eingabe. Bitte wähle eine gültige Option.");
                        break;
                }
            }
        }

        /// <summary>
        /// Fügt ein neues Produkt hinzu. Die Datenbank generiert dabei automatisch die ProductNumber.
        /// </summary>
        private void AddProduct()
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
        /// Sucht nach Produkten anhand des Namens oder zeigt alle an.
        /// </summary>
        private void SearchProduct()
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
        /// Aktualisiert den Bestand eines Produkts anhand der Produktnummer.
        /// </summary>
        private void UpdateProductStock()
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
        /// Exportiert Produkte in ein gewähltes Format (Excel oder Text). Dabei wird die ProductNumber nicht berücksichtigt.
        /// </summary>
        public void ExportProducts()
        {
            var products = GetProducts().ToList();
            if (!products.Any())
            {
                Console.WriteLine("Keine Produkte vorhanden, nichts zu exportieren.");
                return;
            }

            Console.WriteLine("\nWähle das Export-Format: [1] Excel, [2] Text");
            var choice = Console.ReadLine();

            Console.Write("Gib das Verzeichnis an, in dem die Datei gespeichert werden soll: ");
            var directoryPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                Console.WriteLine("Kein Verzeichnis angegeben.");
                return;
            }

            string datePart = DateTime.Now.ToString("dd-MM-yyyy");
            string fileName;
            string finalPath;

            if (choice == "1")
            {
                fileName = $"Products_{datePart}.xlsx";
                finalPath = System.IO.Path.Combine(directoryPath, fileName);
                ExcelExport.ToExcel(products, finalPath);
            }
            else if (choice == "2")
            {
                fileName = $"Products_{datePart}.txt";
                finalPath = System.IO.Path.Combine(directoryPath, fileName);
                ListExport.ToText(products, finalPath);
            }
            else
            {
                Console.WriteLine("Ungültige Auswahl.");
            }
        }

        /// <summary>
        /// Importiert Produkte aus einem gewählten Format (Excel oder Text), ignoriert die ProductNumber
        /// und prüft anhand des Namens, ob das Produkt bereits existiert.
        /// </summary>
        public void ImportProducts()
        {
            Console.WriteLine("\nWähle das Import-Format: [1] Excel, [2] Text");
            var choice = Console.ReadLine();
            Console.Write("Gib den Dateipfad ein (z.B. C:\\Users\\Marko\\Downloads\\Produkte.xlsx): ");
            var filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Kein Dateipfad angegeben.");
                return;
            }

            List<Product> products;
            if (choice == "1")
            {
                products = ExcelImport.FromExcel(filePath);
            }
            else if (choice == "2")
            {
                products = ListImport.FromText(filePath);
            }
            else
            {
                Console.WriteLine("Ungültige Auswahl.");
                return;
            }

            if (products == null || !products.Any())
            {
                Console.WriteLine("Keine Produkte importiert.");
                return;
            }

            foreach (var product in products)
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.Productname == product.Productname);
                if (existingProduct == null)
                {
                    _context.Products.Add(product);
                }
                else
                {
                    existingProduct.Productcategory = product.Productcategory;
                    existingProduct.Price = product.Price;
                    existingProduct.Stock = product.Stock;
                }
            }

            _context.SaveChanges();
            Console.WriteLine("Import abgeschlossen. Bereits vorhandene Produkte (gleicher Name) wurden aktualisiert.");
        }

        /// <summary>
        /// Gibt alle Produkte aus der Datenbank zurück.
        /// </summary>
        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.ToList();
        }
    }
}
