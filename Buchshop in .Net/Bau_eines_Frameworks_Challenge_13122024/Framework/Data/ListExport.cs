using System;
using System.Collections.Generic;
using System.IO;
using BuchShop.Models;

namespace BuchShop.Framework.Data
{
    public static class ListExport
    {
        public static void ToText(List<Product> products, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Keine Kopfzeile – oder du könntest eine einfügen, wenn du magst
                // writer.WriteLine("Name;Kategorie;Preis;Bestand");

                foreach (var product in products)
                {
                    // Nur Name;Kategorie;Preis;Bestand
                    writer.WriteLine($"{product.Productname};{product.Productcategory};{product.Price};{product.Stock}");
                }
            }
            Console.WriteLine("Produktliste wurde erfolgreich als Textdatei exportiert (ohne ProductNumber).");
        }
    }
}
