using MenuSystem;

Console.WriteLine("Hello, tic-tac-toe!");

var menu0 = new Menu ("Tic-Tac-Toe main menu", EMenuLevel.Root);
menu0.AddMenuItem("a", "New game", () =>
{
    //GameStart();
    return "";
});

var menu1 = new Menu("Tic-Tac-Toe Level1 menu", EMenuLevel.FirstLevel);
menu1.AddMenuItem("a", "Level 1 - Option A, returns b", () =>
{
    Console.WriteLine("Level 1 - option A was called");
    return "b";
});

var menu2 = new Menu("Tic-Tac-Toe Level2 menu", EMenuLevel.Other);
menu2.AddMenuItem("a", "Level 2 - Option A, returns m", () =>
{
    Console.WriteLine("Level 2 - option A was called");
    return "m";
});

var menu3 = new Menu("Tic-Tac-Toe Level3 menu", EMenuLevel.Other);
menu3.AddMenuItem("a", "Level 2 - Option A, returns z", () =>
{
    Console.WriteLine("Level 3 - option A was called");
    return "z";
});

menu0.AddMenuItem("1", "Level0 - Go to level1", menu1.Run);
menu1.AddMenuItem("2", "Level1 - Go to level2", menu2.Run);
menu2.AddMenuItem("3", "Level2 - Go to level3", menu3.Run);

menu0.Run();
Console.WriteLine("FINISHED");
