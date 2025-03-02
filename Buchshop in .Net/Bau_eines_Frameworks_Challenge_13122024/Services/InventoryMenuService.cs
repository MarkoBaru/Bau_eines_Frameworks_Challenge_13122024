using System;
using BuchShop.Models;
using BuchShop.Framework.Data;

namespace BuchShop.Services
{
    /// <summary>
    /// Zuständig für die Menüführung der Lagerverwaltung.
    /// Ruft bei Bedarf die passenden Methoden aus den anderen Services auf.
    /// </summary>
    public class InventoryMenuService
    {
        private readonly InventoryDataService _dataService;
        private readonly ImportExportService _importExportService;

        public InventoryMenuService(InventoryDataService dataService, ImportExportService importExportService)
        {
            _dataService = dataService;
            _importExportService = importExportService;
        }

        /// <summary>
        /// Einstiegspunkt für die Lagerverwaltung.
        /// </summary>
        public void ManageInventory()
        {
            while (true)
            {
                ShowInventoryMenu();
                string choice = Console.ReadLine() ?? string.Empty;

                // Wenn HandleInventoryChoice = false -> zurück zum Hauptmenü
                if (!HandleInventoryChoice(choice))
                    return;
            }
        }

        /// <summary>
        /// Zeigt das Lagerverwaltungs-Menü an.
        /// </summary>
        private void ShowInventoryMenu()
        {
            Console.WriteLine("\nLagerverwaltung:");
            Console.WriteLine("[1] Neues Produkt hinzufügen");
            Console.WriteLine("[2] Produkt suchen");
            Console.WriteLine("[3] Produktbestand aktualisieren");
            Console.WriteLine("[4] Produkte exportieren");
            Console.WriteLine("[5] Produkte importieren");
            Console.WriteLine("[6] Zurück zum Hauptmenü");
        }

        /// <summary>
        /// Verarbeitet die Menüauswahl und ruft die entsprechenden Methoden auf.
        /// Gibt false zurück, wenn der User "Zurück" wählt.
        /// </summary>
        private bool HandleInventoryChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    _dataService.AddProduct();
                    break;
                case "2":
                    _dataService.SearchProduct();
                    break;
                case "3":
                    _dataService.UpdateProductStock();
                    break;
                case "4":
                    _importExportService.ExportProducts();
                    break;
                case "5":
                    _importExportService.ImportProducts();
                    break;
                case "6":
                    Console.WriteLine("Zurück zum Hauptmenü...");
                    return false;
                default:
                    Console.WriteLine("Ungültige Eingabe. Bitte wähle eine gültige Option.");
                    break;
            }
            return true;
        }
    }
}
