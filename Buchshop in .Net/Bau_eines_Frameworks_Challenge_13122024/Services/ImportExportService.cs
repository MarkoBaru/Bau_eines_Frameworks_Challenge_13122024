using System;
using System.Collections.Generic;
using System.IO;
using BuchShop.Framework.Data;
using BuchShop.Models;

namespace BuchShop.Services
{
    /// <summary>
    /// Verantwortlich für das Importieren und Exportieren von Produktdaten.
    /// Ruft ExcelImport, ExcelExport, ListImport, ListExport auf
    /// und nutzt InventoryDataService für Datenbankzugriffe.
    /// </summary>
    public class ImportExportService
    {
        private readonly InventoryDataService _dataService;

        public ImportExportService(InventoryDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Exportiert Produkte in Excel oder Textdatei.
        /// </summary>
        public void ExportProducts()
        {
            var products = _dataService.GetAllProducts();
            if (products.Count == 0)
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
                finalPath = Path.Combine(directoryPath, fileName);

                ExcelExport.ToExcel(products, finalPath);
            }
            else if (choice == "2")
            {
                fileName = $"Products_{datePart}.txt";
                finalPath = Path.Combine(directoryPath, fileName);

                ListExport.ToText(products, finalPath);
            }
            else
            {
                Console.WriteLine("Ungültige Auswahl.");
            }
        }

        /// <summary>
        /// Importiert Produkte aus Excel oder Textdatei und aktualisiert/fügt sie in die DB ein.
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

            if (products == null || products.Count == 0)
            {
                Console.WriteLine("Keine Produkte importiert.");
                return;
            }

            // Für jedes importierte Produkt: AddOrUpdateByName
            foreach (var product in products)
            {
                _dataService.AddOrUpdateByName(product);
            }

            // Am Ende alles speichern
            _dataService.SaveChanges();

            Console.WriteLine("Import abgeschlossen. Bereits vorhandene Produkte (gleicher Name) wurden aktualisiert.");
        }
    }
}
