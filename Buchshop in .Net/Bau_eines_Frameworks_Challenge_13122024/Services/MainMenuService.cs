using BuchShop.Models;
using BuchShop.Services;

public class MainMenuService
{
    private readonly InventoryService _inventoryService;
    private readonly ShoppingService _shoppingService;
    private readonly AuthenticationService _authService;

    public MainMenuService(
        InventoryService inventoryService,
        ShoppingService shoppingService,
        AuthenticationService authService)
    {
        _inventoryService = inventoryService;
        _shoppingService = shoppingService;
        _authService = authService;
    }

    public void DisplayMainMenu()
    {
        while (true)
        {
            Console.WriteLine("\nWähle eine Option:");
            Console.WriteLine("[1] Einloggen");
            Console.WriteLine("[2] Registrieren");
            Console.WriteLine("[3] Programm beenden");

            var choice = Console.ReadLine() ?? string.Empty;

            switch (choice)
            {
                case "1":
                    LoginUser();
                    break;
                case "2":
                    RegisterUser();
                    break;
                case "3":
                    Console.WriteLine("Programm beendet.");
                    return;
                default:
                    Console.WriteLine("Ungültige Eingabe. Versuche es erneut.");
                    break;
            }
        }
    }

    private void LoginUser()
    {
        // AuthenticationService übernimmt den Login-Prozess inklusive Begrüssung,
        // daher erfolgt hier nur der Aufruf und dann der Übergang ins Benutzer-Menü.
        var user = _authService.Login();
        if (user != null)
        {
            // Keine zusätzliche Begrüssung hier, um doppelte Ausgaben zu vermeiden.
            DisplayUserMenu(user);
        }
        else
        {
            Console.WriteLine("Login fehlgeschlagen. Bitte versuche es erneut.");
        }
    }

    private void RegisterUser()
    {
        // Registrierung wird ausschliesslich durch AuthenticationService durchgeführt.
        _authService.RegisterNewUser();
    }

    private void DisplayUserMenu(User loggedInUser)
    {
        while (true)
        {
            Console.WriteLine("\nProdukte:");
            var products = _inventoryService.GetProducts();

            if (!products.Any())
            {
                Console.WriteLine("Keine Produkte verfügbar.");
            }
            else
            {
                foreach (var product in products)
                {
                    Console.WriteLine($"- {product.Productname} ({product.Productcategory}): {product.Price:C}");
                }
            }

            Console.WriteLine("\nWas möchtest du tun?");
            Console.WriteLine("[1] Einkaufen");
            Console.WriteLine("[2] Zur Lagerverwaltung");
            Console.WriteLine("[3] Abmelden");

            var choice = Console.ReadLine() ?? string.Empty;

            switch (choice)
            {
                case "1":
                    if (products.Any())
                    {
                        Console.WriteLine("Einkaufen gestartet...");
                        _shoppingService.StartShopping(loggedInUser);
                    }
                    else
                    {
                        Console.WriteLine("Es gibt keine Produkte zu kaufen.");
                    }
                    break;
                case "2":
                    Console.WriteLine("Zur Lagerverwaltung gewechselt...");
                    _inventoryService.ManageInventory();
                    break;
                case "3":
                    Console.WriteLine("Abgemeldet.");
                    return;
                default:
                    Console.WriteLine("Ungültige Eingabe. Versuche es erneut.");
                    break;
            }
        }
    }
}
