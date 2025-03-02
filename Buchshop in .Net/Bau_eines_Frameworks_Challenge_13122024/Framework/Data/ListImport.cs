using System;
using System.Collections.Generic;
using System.IO;
using BuchShop.Models;

namespace BuchShop.Framework.Data
{
    public static class ListImport
    {
        public static List<Product> FromText(string filePath)
        {
            List<Product> products = new List<Product>();

            // Prüfen, ob die Datei existiert
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Fehler: Die Datei '{filePath}' wurde nicht gefunden.");
                return products;
            }

            try
            {
                // Alle Zeilen einlesen
                var lines = File.ReadAllLines(filePath);
                if (lines.Length == 0)
                {
                    Console.WriteLine("Fehler: Die Textdatei ist leer.");
                    return products;
                }

                foreach (var line in lines)
                {
                    var parts = line.Split(';');

                    // Erwartet 4 Felder: Name, Kategorie, Preis, Bestand
                    if (parts.Length == 4)
                    {
                        var name = parts[0];
                        var category = parts[1];
                        var priceStr = parts[2];
                        var stockStr = parts[3];

                        if (!decimal.TryParse(priceStr, out decimal price))
                        {
                            Console.WriteLine($"Ungültiger Preis in Zeile: {line}");
                            continue;
                        }

                        if (!int.TryParse(stockStr, out int stock))
                        {
                            Console.WriteLine($"Ungültiger Bestand in Zeile: {line}");
                            continue;
                        }

                        products.Add(new Product
                        {
                            Productname = name,
                            Productcategory = category,
                            Price = price,
                            Stock = stock
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Zeile ungültig (erwartet 4 Felder): {line}");
                    }
                }

                Console.WriteLine("Produktliste wurde erfolgreich aus der Textdatei importiert (ohne ProductNumber).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Import aus Textdatei: {ex.Message}");
            }

            return products;
        }
    }
}
