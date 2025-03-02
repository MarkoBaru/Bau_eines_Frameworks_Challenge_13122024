using System;
using System.Collections.Generic;
using System.IO;
using BuchShop.Models;
using OfficeOpenXml;

namespace BuchShop.Framework.Data
{
    public static class ExcelImport
    {
        public static List<Product> FromExcel(string filePath)
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
                using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Prüfen, ob es mindestens ein Worksheet gibt
                    if (package.Workbook.Worksheets.Count == 0)
                    {
                        Console.WriteLine("Fehler: Die Excel-Datei enthält keine Tabellenblätter.");
                        return products;
                    }

                    // Erstes Worksheet nehmen
                    var worksheet = package.Workbook.Worksheets[0];

                    // Prüfen, ob das Worksheet überhaupt Daten hat
                    if (worksheet.Dimension == null)
                    {
                        Console.WriteLine("Fehler: Das Tabellenblatt ist leer oder nicht korrekt formatiert.");
                        return products;
                    }

                    // Anzahl der Zeilen ermitteln
                    int rowCount = worksheet.Dimension.Rows;
                    if (rowCount < 2)
                    {
                        Console.WriteLine("Fehler: Die Excel-Datei enthält keine Datenzeilen.");
                        return products;
                    }

                    // Erwartete Spalten (ab Zeile 2):
                    // 1: Name, 2: Kategorie, 3: Preis, 4: Bestand
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var nameCell = worksheet.Cells[row, 1].Value?.ToString();
                        var categoryCell = worksheet.Cells[row, 2].Value?.ToString();
                        var priceCell = worksheet.Cells[row, 3].Value?.ToString();
                        var stockCell = worksheet.Cells[row, 4].Value?.ToString();

                        // Einfache Validierung
                        if (string.IsNullOrWhiteSpace(nameCell) ||
                            string.IsNullOrWhiteSpace(categoryCell) ||
                            string.IsNullOrWhiteSpace(priceCell) ||
                            string.IsNullOrWhiteSpace(stockCell))
                        {
                            Console.WriteLine($"Fehler in Zeile {row}: Mindestens eine Zelle ist leer.");
                            continue;
                        }

                        // Parsen
                        if (!decimal.TryParse(priceCell, out decimal price))
                        {
                            Console.WriteLine($"Fehler in Zeile {row}: Ungültiger Preis '{priceCell}'.");
                            continue;
                        }

                        if (!int.TryParse(stockCell, out int stock))
                        {
                            Console.WriteLine($"Fehler in Zeile {row}: Ungültiger Bestand '{stockCell}'.");
                            continue;
                        }

                        // Neue Produkt-Instanz (ohne ProductNumber, DB generiert Identity)
                        products.Add(new Product
                        {
                            Productname = nameCell,
                            Productcategory = categoryCell,
                            Price = price,
                            Stock = stock
                        });
                    }
                }
                Console.WriteLine("Produktliste wurde erfolgreich aus der Excel-Datei importiert (ohne ProductNumber).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Import aus Excel: {ex.Message}");
            }

            return products;
        }
    }
}
