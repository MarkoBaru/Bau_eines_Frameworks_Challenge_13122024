using System;
using System.Collections.Generic;
using System.IO;
using BuchShop.Models;
using OfficeOpenXml;

namespace BuchShop.Framework.Data
{
    public static class ExcelExport
    {
        public static void ToExcel(List<Product> products, string filePath)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                // Neues Worksheet "Produkte" anlegen
                var worksheet = package.Workbook.Worksheets.Add("Produkte");

                // Kopfzeilen anlegen (ohne ProductNumber)
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Kategorie";
                worksheet.Cells[1, 3].Value = "Preis";
                worksheet.Cells[1, 4].Value = "Bestand";

                // Daten ab Zeile 2 schreiben
                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = products[i].Productname;
                    worksheet.Cells[i + 2, 2].Value = products[i].Productcategory;
                    worksheet.Cells[i + 2, 3].Value = products[i].Price;
                    worksheet.Cells[i + 2, 4].Value = products[i].Stock;
                }

                File.WriteAllBytes(filePath, package.GetAsByteArray());
                Console.WriteLine("Produktliste wurde erfolgreich als Excel-Datei exportiert (ohne ProductNumber).");
            }
        }
    }
}
