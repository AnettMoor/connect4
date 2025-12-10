using BLL;
using ConsoleApp;
using DAL;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
 
GameController? lastController = null;
GameConfiguration? lastGameConfig = null;
string? lastLoadedFileName = null;

Console.WriteLine("Hello, Connect4!");


IRepository<GameConfiguration> configRepo;

// Choose ONE!
//configRepo = new ConfigRepositoryJson();

using var dbContext = GetDbContext();
configRepo = new ConfigRepositoryEF(dbContext);


var menu0 = new Menu("Connect4 Main Menu", EMenuLevel.Root);
menu0.AddMenuItem("n", "New game", () =>
{
    lastController = new GameController();
    lastGameConfig = lastController.GetConfiguration();
    //mid game saving
    midGameSave();
    lastController.GameLoop();
    
    // Store game state for later saving - after game saving
    lastGameConfig = lastController.GetConfiguration();

    return "abc";
});

var menuConfig = new Menu("Connect4 Configurations", EMenuLevel.FirstLevel);
menuConfig.AddMenuItem("l", "Load", () =>
{
    var data = configRepo.List();
    if (data.Count == 0)
    {
        Console.WriteLine("No saved configurations found.");
        return "abc";
    }

    for (int i = 0; i < data.Count; i++)
        Console.WriteLine($"{i + 1}: {data[i].description}");

    Console.Write("Select config to load, 0 to skip: ");
    if (!int.TryParse(Console.ReadLine(), out int userChoice) || userChoice <= 0 || userChoice > data.Count)
    {
        Console.WriteLine("Skipped.");
        return "abc";
    }

    var selectedId = data[userChoice - 1].id;
    var gameConfig = configRepo.Load(selectedId);
    
    // Convert saved board list to 2D array if it exists
    ECellState[,] loadedBoard = new ECellState[gameConfig.BoardWidth, gameConfig.BoardHeight];
    if (gameConfig.Board != null)
    {
        for (int x = 0; x < gameConfig.BoardWidth; x++)
        for (int y = 0; y < gameConfig.BoardHeight; y++)
            loadedBoard[x, y] = gameConfig.Board[x][y];
    }

    // Start the game with the loaded configuration
    lastController = new GameController(gameConfig, "Player 1", "Player 2", loadedBoard);
    lastGameConfig = gameConfig;
    lastLoadedFileName = selectedId;
    
    midGameSave();
    lastController.GameLoop();
    
    return "m";
});


menuConfig.AddMenuItem("e", "Edit", () =>
{
    {
        var data = configRepo.List();
        for (int i = 0; i < data.Count; i++)
        {
            var (id, description) = data[i];
            Console.WriteLine($"{i + 1}: {description}");
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
    var newConfig = new GameConfiguration() { Name = "Classical" };
    configRepo.Save(newConfig);
    Console.WriteLine($"New configuration created: {newConfig.Name}");
    return "abc";
});

menuConfig.AddMenuItem("d", "Delete", () =>
{
    
    var data = configRepo.List();
    for (int i = 0; i < data.Count; i++)
    {
        var (id, description) = data[i];
        Console.WriteLine($"{i + 1}: {description}");
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

menu0.AddMenuItem("s", "Save game", () =>
{
    if (lastGameConfig == null)
    {
        Console.WriteLine("No game to save. Play a game first.");
        return "abc";
    }

    if (lastController != null && !lastController.GameSaved)
    {
        lastController.UpdateConfigurationBoard();
        
        Console.Write("Enter a name for this saved game: ");
        var name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name))
            lastGameConfig.Name = name;
        configRepo.Save(lastGameConfig);
        Console.WriteLine($"Game configuration saved: {lastGameConfig.Name}");
    }
    else
    {
        Console.WriteLine($"Game already saved: {lastGameConfig.Name}");
    }
    return "abc";
});


menu0.Run();

Console.WriteLine("We are DONE.......");

AppDbContext GetDbContext()
{
    // ========================= DB STUFF ========================
    var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    homeDirectory = homeDirectory + Path.DirectorySeparatorChar;

    // We are using SQLite
    var connectionString = $"Data Source={homeDirectory}connect4.db";

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


void midGameSave()
{
    if (lastController == null) return; // only check controller

    lastController.OnSaveGame = (gameConfig) =>
    {
        lastController.UpdateConfigurationBoard(); // sync board

        // give new games name
        if (string.IsNullOrWhiteSpace(gameConfig.Name))
        {
            Console.Write("Enter a name for this saved game: ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
                gameConfig.Name = name;
        }

        if (!string.IsNullOrWhiteSpace(lastLoadedFileName))
        {
            // Override loaded game
            configRepo.Update(gameConfig, lastLoadedFileName);
            Console.WriteLine($"Loaded game overridden: {gameConfig.Name}");
        }
        else
        {
            // New game save
            configRepo.Save(gameConfig);
            Console.WriteLine($"Game saved: {gameConfig.Name}");
        }

        lastGameConfig = gameConfig; // now we can store it
    };


}
