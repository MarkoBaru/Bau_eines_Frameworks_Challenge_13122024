using System.Text.RegularExpressions;

namespace BuchShop.Framework.Controls
{
    /// <summary>
    /// Enthält eine statische Methode, um E-Mail-Adressen zu validieren.
    /// </summary>
    public static class EmailValidation
    {
        /// <summary>
        /// Prüft, ob eine E-Mail-Adresse formal gültig ist.
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            // Leere oder null-Eingaben sind ungültig
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Einfacher Regex für typische E-Mail-Formate
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }
}
