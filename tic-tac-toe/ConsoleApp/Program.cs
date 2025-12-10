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
    
    // Convert saved board list to list of lists if it exists
    List<List<ECellState>> loadedBoard = new List<List<ECellState>>();
    
    for (int x = 0; x < gameConfig.BoardWidth; x++)
    {
        List<ECellState> col = new List<ECellState>();

        if (gameConfig.Board != null && x < gameConfig.Board.Count)
        {
            // copy existing rows
            col.AddRange(gameConfig.Board[x]);

            // fill missing rows
            while (col.Count < gameConfig.BoardHeight)
                col.Add(ECellState.Empty);
        }
        else
        {
            // create empty column
            for (int y = 0; y < gameConfig.BoardHeight; y++)
                col.Add(ECellState.Empty);
        }

        loadedBoard.Add(col);
    }

// now create GameController safely
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
        gameConfig.BoardWidth = ReadInt("Enter new board width: ");
        gameConfig.BoardHeight = ReadInt("Enter new board height: ");

        // save new name
        var newFileName = configRepo.Update(gameConfig, selectedConfig.id);

        Console.WriteLine($"Config updated. New file: {newFileName}");
        return "abc";
    }
});

menuConfig.AddMenuItem("c", "Create Custom", () =>
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



void PrecreatedConfigs()
{
    var existingConfigs = configRepo.List();
    void AddOrFixConfig(string name, int width, int height, int winCondition)
    {
        var existingEntity = dbContext.GameConfigurations
            .FirstOrDefault(c => c.Name == name);

        GameConfiguration config;

        if (existingEntity != null)
        {
            // load and update for existing boards
            config = configRepo.Load(existingEntity.Id.ToString());
            configRepo.Update(config, existingEntity.Id.ToString());
        }
        else
        {
            // Create and save
            config = new GameConfiguration
            {
                Name = name,
                BoardWidth = width,
                BoardHeight = height,
                WinCondition = winCondition
            };

            // Initialize empty board
            config.Board = new List<List<ECellState>>();
            for (int x = 0; x < config.BoardWidth; x++)
            {
                config.Board.Add(new List<ECellState>());
                while (config.Board[x].Count < config.BoardHeight)
                    config.Board[x].Add(ECellState.Empty);
            }
            // save if not existing already
            configRepo.Save(config);
        }
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
    if (lastController == null) return; // only check controller

    lastController.OnSaveGame = (gameConfig) =>
    {
        lastController.UpdateConfigurationBoard();

        // give new games name
        if (string.IsNullOrWhiteSpace(gameConfig.Name))
        {
            Console.Write("Enter a name for this saved game: ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
                gameConfig.Name = name;
        }
        
        bool dbGameExists = false;
        if (!string.IsNullOrWhiteSpace(lastLoadedFileName))
        {
            // Check if ID exists in DB before updating
            var allConfigs = configRepo.List();
            dbGameExists = allConfigs.Any(c => c.id == lastLoadedFileName);
        }

        if (dbGameExists)
        {
            configRepo.Update(gameConfig, lastLoadedFileName);
            Console.WriteLine($"Loaded game overridden: {gameConfig.Name}");
        }
        else
        {
            configRepo.Save(gameConfig);
            Console.WriteLine($"Game saved: {gameConfig.Name}");
            lastLoadedFileName = null;
        }

        lastGameConfig = gameConfig;
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

