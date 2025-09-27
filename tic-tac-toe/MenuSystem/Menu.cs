namespace MenuSystem;

public class Menu
{
    private string Title { get; set; } = default!;
    private Dictionary<string, MenuItem> MenuItems { get; set; } = new();
    
    private EMenuLevel Level { get; set; }
    
    //basic menu functions
    private MenuItem x = new MenuItem() { Key = "x", Value = "Return to previous (Exit)" };
    private MenuItem m = new MenuItem()  { Key = "m", Value = "Return to main menu" };
    private MenuItem b = new MenuItem() { Key = "b", Value = "Back to previous menu" };
    
    public void AddMenuItem(string key, string value, Func<string> methodToRun)
    {
        if (MenuItems.ContainsKey(key))
        {
            throw new ArgumentException($"menu item {key} already exists");
        }
        MenuItems[key] = new MenuItem() { Key = key, Value = value, MethodToRun = methodToRun };
    }
    
    public Menu(string title, EMenuLevel level)
    {
        Title = title;
        Level = level;

        switch (level)
        {
            case EMenuLevel.Root:
                MenuItems[x.Key] = x;
                break;
            case EMenuLevel.FirstLevel:
                MenuItems[m.Key] = m;
                MenuItems[x.Key] = x;
                break;
            case EMenuLevel.Other:
                MenuItems[b.Key] = b;
                MenuItems[m.Key] = m;
                MenuItems[x.Key] = x;
                break;
        }
    }

    public string Run()
    {
        Console.Clear();
        var menuRunning = true;
        var userChoice = "";

        do
        {
            DisplayMenu();
            Console.Write("Select and option: ");

            var input = Console.ReadLine();
            if (input == null)
            {
                Console.WriteLine("invalid input");
                continue;
            }

            userChoice = input.Trim().ToLower();
            
            // is userchoice valid on current level checks
            if (!ChoiceIsValidOnThisLevel(userChoice))
            {
                Console.WriteLine($"'{userChoice}' is not valid at this menu level.");
                continue;
            }
            
            if (userChoice == "x" || userChoice == "m" || userChoice == "b")
            {
                menuRunning = false;
            }
            else
            {
                if (MenuItems.ContainsKey(userChoice))
                {
                    var returnValueFromMethodToRun = MenuItems[userChoice].MethodToRun?.Invoke();
                    if (returnValueFromMethodToRun == "m" && Level != EMenuLevel.Other)
                    {
                        menuRunning = false;
                        userChoice = "m";
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
            Console.WriteLine();
        } while (menuRunning);

        return userChoice;
    }

    private void DisplayMenu()
    {
        Console.WriteLine(Title);
        Console.WriteLine("--------------------");
        
        // printing out the menu in the correct order
        foreach (var item in MenuItems.Values.Where(i => i.Key != "x" && i.Key != "m" && i.Key != "b"))
        {
            Console.WriteLine(item);
        }
        if (MenuItems.ContainsKey("b")) Console.WriteLine(MenuItems["b"]);
        if (MenuItems.ContainsKey("m")) Console.WriteLine(MenuItems["m"]);
        Console.WriteLine(MenuItems["x"]);
        
    }

    private bool ChoiceIsValidOnThisLevel(string choice)
    {
        // root level - b, m not allowed
        if (Level == EMenuLevel.Root && (choice == "m" || choice == "b"))
        {
            return false;
        }
        // first level - b not allowed
        if (Level == EMenuLevel.FirstLevel && choice == "b")
        {
            return false;
        }
        // third level, everything allowed
        return true;
    }
}