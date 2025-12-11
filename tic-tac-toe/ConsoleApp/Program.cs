using BLL;
using ConsoleApp;
using DAL;
using DAL.Migrations;
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
menu0.AddMenuItem("n", "New game (Classic)", () =>
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

menuConfig.AddMenuItem("t", "Load premade template", () =>
{
    // Get list of template configs
    var allConfigs = configRepo.List(); 
    var templates = allConfigs
        .Select(c => configRepo.Load(c.id))  
        .Where(c => c.IsTemplate) // load only templates
        .ToList();

    if (templates.Count == 0)
    {
        Console.WriteLine("No templates found.");
        return "abc";
    }

    for (int i = 0; i < templates.Count; i++)
        Console.WriteLine($"{i + 1}: {templates[i].Name} ({templates[i].BoardWidth}x{templates[i].BoardHeight})");

    Console.Write("Select template to start, 0 to skip: ");
    if (!int.TryParse(Console.ReadLine(), out int choice) || choice <= 0 || choice > templates.Count)
    {
        Console.WriteLine("Skipped.");
        return "abc";
    }

    var selected = templates[choice - 1];

    // Always create a controller from a copy to avoid modifying the template
    var boardCopy = selected.Board != null
        ? selected.Board.Select(col => new List<ECellState>(col)).ToList()
        : null;
    
    var gameCopy = new GameConfiguration
    {
        Id = Guid.NewGuid(),
        Name = selected.Name,
        BoardWidth = selected.BoardWidth,
        BoardHeight = selected.BoardHeight,
        WinCondition = selected.WinCondition,
        Board = boardCopy,
        IsTemplate = false
    };

    lastController = new GameController(gameCopy, "Player 1", "Player 2", boardCopy);

    lastGameConfig = gameCopy;
    lastLoadedFileName = null;

    midGameSave();
    lastController.GameLoop();

    return "abc";
});

menuConfig.AddMenuItem("l", "Load saved game", () =>
{
    var data = configRepo.List()
        .Select(c => configRepo.Load(c.id)) // load full GameConfiguration
        .Where(c => !c.IsTemplate) // exclude templates
        .ToList();
    if (data.Count == 0)
    {
        Console.WriteLine("No saved configurations found.");
        return "abc";
    }

    for (int i = 0; i < data.Count; i++)
        Console.WriteLine($"{i + 1}: {data[i].Name}");

    Console.Write("Select config to load, 0 to skip: ");
    if (!int.TryParse(Console.ReadLine(), out int userChoice) || userChoice <= 0 || userChoice > data.Count)
    {
        Console.WriteLine("Skipped.");
        return "abc";
    }

    var selected = data[userChoice - 1];

    // Deep copy the board to avoid unexpected changes
    var boardCopy = selected.Board != null
        ? selected.Board.Select(col => new List<ECellState>(col)).ToList()
        : null;

    lastController = new GameController(selected, "Player 1", "Player 2", boardCopy);

    lastGameConfig = selected;
    lastLoadedFileName = selected.Id.ToString();

    midGameSave();
    lastController.GameLoop();

    return "abc";
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
        gameConfig.BoardWidth = ReadInt("Enter new board width: ");
        gameConfig.BoardHeight = ReadInt("Enter new board height: ");

        // save new name
        var newFileName = configRepo.Update(gameConfig, selectedConfig.id);

        Console.WriteLine($"Config updated. New file: {newFileName}");
        return "abc";
    }
});

menuConfig.AddMenuItem("c", "Create custom game", () =>
{
    var newConfig = new GameConfiguration();
    Console.Write("Enter new name: ");
    newConfig.Name = Console.ReadLine();

    newConfig.BoardWidth = ReadInt("Enter new board width: ");
    newConfig.BoardHeight = ReadInt("Enter new board height: ");
    newConfig.WinCondition = ReadInt("Win Condition: ");

    newConfig.Board = null;
    
    configRepo.Save(newConfig);
    Console.WriteLine($"New configuration created: {newConfig.Name}");
    
    lastController = new GameController(newConfig, "Player 1", "Player 2");
    lastGameConfig = newConfig;
    lastLoadedFileName = null; // new config, not loaded from DB
    
    midGameSave();
    
    lastController.GameLoop();
    return "abc";
});

menuConfig.AddMenuItem("d", "Delete", () =>
{
    // Load saved games only, exclude templates
    var data = configRepo.List()
        .Select(c => configRepo.Load(c.id))
        //.Where(c => !c.IsTemplate)
        .ToList();

    if (data.Count == 0)
    {
        Console.WriteLine("No saved games to delete.");
        return "abc";
    }

    for (int i = 0; i < data.Count; i++)
    {
        Console.WriteLine($"{i + 1}: {data[i].Name} ({data[i].BoardWidth}x{data[i].BoardHeight})");
    }

    Console.Write("Select config to delete, 0 to skip: ");
    var userChoiceStr = Console.ReadLine();
    if (!int.TryParse(userChoiceStr, out int userChoice) || userChoice <= 0 || userChoice > data.Count)
    {
        Console.WriteLine("Skipped.");
        return "abc";
    }

    var selectedConfig = data[userChoice - 1];

    // Delete using repository (BLL)
    configRepo.Delete(selectedConfig.Id.ToString());

    Console.WriteLine($"Deleted: {selectedConfig.Name}");
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

    lastController?.UpdateConfigurationBoard();

    // Check if config already exists in DB
    var allConfigs = configRepo.List();
    bool existsInDb = allConfigs.Any(c => c.id == lastGameConfig.Id.ToString());

    if (existsInDb && lastGameConfig.IsTemplate == false)
    {
        // if game already exists, update
        configRepo.Update(lastGameConfig, lastGameConfig.Id.ToString());
        Console.WriteLine($"Game configuration updated: {lastGameConfig.Name}");
    }
    else
    {
        // New saves and premade config plays
        lastGameConfig.Id = Guid.NewGuid();
        Console.Write("Enter name for the game: ");
        lastGameConfig.Name = Console.ReadLine();
        configRepo.Save(lastGameConfig);
        Console.WriteLine($"Game configuration saved: {lastGameConfig.Name}");
    }

    return "abc";
});



void PrecreatedConfigs()
{
    bool NameExists(string name)
    {
        var existingConfigs = configRepo.List();
        return existingConfigs.Any(c =>
        {
            // Name - widthxheight - win_X - createdat
            var parts = c.description.Split(" - ", StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 && parts[0] == name;
        });
    }

    void AddOrFixConfig(string name, int width, int height, int winCondition)
    {
        if (NameExists(name))
            return; 

        // if config not existing
        // Create new configuration
        var config = new GameConfiguration
        {
            Id = Guid.NewGuid(),
            Name = name,
            BoardWidth = width,
            BoardHeight = height,
            WinCondition = winCondition,
            IsTemplate =  true,
            Board = new List<List<ECellState>>()
        };

        // Initialize empty board
        for (int x = 0; x < config.BoardWidth; x++)
        {
            var col = new List<ECellState>();
            for (int y = 0; y < config.BoardHeight; y++)
                col.Add(ECellState.Empty);
            config.Board.Add(col);
        }
        // save if not existing already
        configRepo.Save(config);
    }
    AddOrFixConfig("Classical Connect4", 4, 4, 4);
    AddOrFixConfig("Connect3", 5, 5, 3);
    AddOrFixConfig("Connect5", 7, 7, 5);
}

PrecreatedConfigs();
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
    if (lastController == null) return;

    lastController.OnSaveGame = (gameConfig) =>
    {
        lastController.UpdateConfigurationBoard();
        bool isNew = gameConfig.IsTemplate || string.IsNullOrEmpty(lastLoadedFileName);

        // Only save the current game copy
        var allConfigs = configRepo.List();
        bool exists = allConfigs.Any(c => c.id == gameConfig.Id.ToString());

        if (isNew)
        {
            // Prompt for a new name
            Console.Write("Enter name for this saved game: ");
            var newName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newName))
                newName = "Saved Game";

            gameConfig.Name = newName;
            gameConfig.Id = Guid.NewGuid(); // ensure new ID
            gameConfig.IsTemplate = false;   // mark as non-template
            lastLoadedFileName = null;       // mark as new
        }

        // Save or update for nontempaltes
        if (isNew)
        {
            configRepo.Save(gameConfig);
            Console.WriteLine($"Game saved: {gameConfig.Name}");
        }
        else
        {
            configRepo.Update(gameConfig, gameConfig.Id.ToString());
            Console.WriteLine($"Game updated: {gameConfig.Name}");
        }

        lastGameConfig = gameConfig;
        lastLoadedFileName = gameConfig.Id.ToString();
    };
}


int ReadInt(string message)
    {
        int value;
        while (true)
        {
            Console.Write(message);
            var input = Console.ReadLine();

            if (int.TryParse(input, out value) && value > 0)
                return value;

            Console.WriteLine("Invalid number. Please enter a positive number.");
        }
    }

