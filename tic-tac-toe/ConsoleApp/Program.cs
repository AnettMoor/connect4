using BLL;
using ConsoleApp;
using DAL;
using MenuSystem;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, Connect4!");


IRepository<GameConfiguration> configRepo;

// Choose ONE!
configRepo = new ConfigRepositoryJson();

//using var dbContext = GetDbContext();
//configRepo = new ConfigRepositoryEF(dbContext);


var menu0 = new Menu("Connect4 Main Menu", EMenuLevel.Root);
menu0.AddMenuItem("n", "New game", () =>
{
    var controller = new GameController();
    controller.GameLoop();
    return "abc";
});

var menuConfig = new Menu("Connect4 Configurations", EMenuLevel.FirstLevel);
menuConfig.AddMenuItem("l", "Load", () =>
{
    var count = 0;
    var data = configRepo.List();
    foreach (var configName in data)
    {
        Console.WriteLine((count + 1) + ": " + configName);
        count++;
    }
    Console.Write("Select config to load, 0 to skip:");
    var userChoice = Console.ReadLine();
    return "abc";
});
menuConfig.AddMenuItem("e", "Edit", () =>
{

    {
        var data = configRepo.List();
        for (int i = 0; i < data.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {data[i]}");
        }

        Console.Write("Select config to edit, 0 to skip: ");
        var userChoiceStr = Console.ReadLine();
        if (!int.TryParse(userChoiceStr, out int userChoice))
        {
            Console.WriteLine("Invalid input!");
            return "abc";
        }

        if (userChoice <= 0 || userChoice > data.Count)
        {
            Console.WriteLine("Skipped.");
            return "abc";
        }

        var selectedConfig = data[userChoice - 1];
        var gameConfig = configRepo.Load(selectedConfig.id);

        // user modifications
        Console.Write("Enter new name: ");
        gameConfig.Name = Console.ReadLine();

        Console.Write("Enter new board width: ");
        gameConfig.BoardWidth = int.Parse(Console.ReadLine());

        Console.Write("Enter new board height: ");
        gameConfig.BoardHeight = int.Parse(Console.ReadLine());

        // save new name
        var newFileName = configRepo.Update(gameConfig, selectedConfig.id);

        Console.WriteLine($"Config updated. New file: {newFileName}");
        return "abc";
    }
});

menuConfig.AddMenuItem("c", "Create", () =>
{
    configRepo.Save(new GameConfiguration(){Name = "Classical"});
    return "abc";
});

menuConfig.AddMenuItem("d", "Delete", () =>
{
    var data = configRepo.List();
    for (int i = 0; i < data.Count; i++)
    {
        Console.WriteLine($"{i + 1}: {data[i]}");
    }

    Console.Write("Select config to delete, 0 to skip: ");
    var userChoiceStr = Console.ReadLine();
    if (!int.TryParse(userChoiceStr, out int userChoice))
    {
        Console.WriteLine("Invalid input!");
        return "abc";
    }

    if (userChoice <= 0 || userChoice > data.Count)
    {
        Console.WriteLine("Skipped.");
        return "abc";
    }

    var selectedConfig = data[userChoice - 1];
    configRepo.Delete(selectedConfig.id);
    Console.WriteLine($"Deleted: {selectedConfig}");

    return "abc";
});


menu0.AddMenuItem("c", "Game Configurations", menuConfig.Run);

menu0.Run();

Console.WriteLine("We are DONE.......");

AppDbContext GetDbContext()
{
    // ========================= DB STUFF ========================
    var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    homeDirectory = homeDirectory + Path.DirectorySeparatorChar;

// We are using SQLite
    var connectionString = $"Data Source={homeDirectory}app.db";

    var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(connectionString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        //.LogTo(Console.WriteLine)
        .Options;

    var dbContext = new AppDbContext(contextOptions);
    
    // apply any pending migrations (recreates db as needed)
    dbContext.Database.Migrate();
    
    return dbContext;
}
