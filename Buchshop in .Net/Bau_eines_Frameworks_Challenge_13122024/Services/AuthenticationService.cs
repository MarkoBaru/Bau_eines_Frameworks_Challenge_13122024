using BuchShop.Models;
using BuchShop.Framework.Data;
using BuchShop.Framework.Controls; // <-- Für EmailValidation
using System.Linq;

namespace BuchShop.Services
{
    public class AuthenticationService
    {
        private readonly AppDbContext _context;

        public AuthenticationService(AppDbContext context)
        {
            _context = context;
        }

        public User? Login()
        {
            Console.WriteLine("\nLogin:");
            Console.Write("E-Mail: ");
            string email = Console.ReadLine() ?? string.Empty;

            Console.Write("Passwort: ");
            string password = Console.ReadLine() ?? string.Empty;

            var user = _context.Users
                .FirstOrDefault(u => u.UserEmail == email && u.UserPassword == password);

            if (user != null)
            {
                Console.WriteLine($"Willkommen zurück, {user.Firstname}!");
                return user;
            }
            else
            {
                Console.WriteLine("Ungültige Login-Daten. Bitte versuche es erneut.");
                return null;
            }
        }

        public void RegisterNewUser()
        {
            Console.WriteLine("\nRegistrierung:");

            Console.Write("Vorname: ");
            string firstname = Console.ReadLine() ?? string.Empty;

            Console.Write("Nachname: ");
            string lastname = Console.ReadLine() ?? string.Empty;

            Console.Write("Adresse: ");
            string address = Console.ReadLine() ?? string.Empty;

            // E-Mail so lange abfragen, bis eine gültige Adresse eingegeben wurde
            string email = string.Empty;
            while (true)
            {
                Console.Write("E-Mail: ");
                email = Console.ReadLine() ?? string.Empty;

                // Über EmailValidation prüfen
                if (!EmailValidation.IsValidEmail(email))
                {
                    Console.WriteLine("Die eingegebene E-Mail-Adresse ist ungültig. Bitte erneut eingeben.");
                }
                else
                {
                    break;
                }
            }

            Console.Write("Passwort: ");
            string password = Console.ReadLine() ?? string.Empty;

            // Leere Eingaben prüfen
            if (string.IsNullOrWhiteSpace(firstname) ||
                string.IsNullOrWhiteSpace(lastname) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Alle Felder müssen ausgefüllt sein!");
                return;
            }

            // Überprüfen, ob die E-Mail bereits registriert ist
            if (_context.Users.Any(u => u.UserEmail == email))
            {
                Console.WriteLine("Ein Benutzer mit dieser E-Mail-Adresse existiert bereits.");
                return;
            }

            var newUser = new User
            {
                Firstname = firstname,
                Lastname = lastname,
                Address = address,
                UserEmail = email,
                UserPassword = password
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            Console.WriteLine("Registrierung erfolgreich! Du kannst dich jetzt einloggen.");
        }
    }
}
