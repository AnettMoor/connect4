using BLL;
using ConsoleApp;
using DAL;
using MenuSystem;

Console.WriteLine("Hello, Connect4!");

var menu0 = new Menu("Connect4 Main Menu", EMenuLevel.Root);
menu0.AddMenuItem("n", "New game", () =>
{
    var controller = new GameController();
    controller.GameLoop();
    return "abc";
});

var menuConfig = new Menu("Connect4 Configurations", EMenuLevel.FirstLevel);
var configRepo = new ConfigRepositoryJson();
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
menuConfig.AddMenuItem("e", "Edit", () => { return "abc"; });
menuConfig.AddMenuItem("c", "Create", () =>
{
    configRepo.Save(new GameConfiguration(){Name = "Classical"});
    return "abc";
});

menuConfig.AddMenuItem("d", "Delete", () => { return "abc"; });


menu0.AddMenuItem("c", "Game Configurations", menuConfig.Run);

menu0.Run();

Console.WriteLine("We are DONE.......");