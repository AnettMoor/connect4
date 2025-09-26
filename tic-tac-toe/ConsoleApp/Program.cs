// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design;
using MenuSystem;

Console.WriteLine("Hello, TIC-TAC-TOE");

var mainMenu = new Menu();

mainMenu.AddMenuItems(
[
    new MenuItem
    {
        Label = "Label1",
        Key = "1",
        RunThisThingWhenMenuItemIsSelected = (key) => Console.WriteLine($"You selected {key}"),
    },
    new MenuItem
    {
        Label = "Label2 with same key",
        Key = "1",
        RunThisThingWhenMenuItemIsSelected = DoSomething
    },
    new MenuItem
    {
    Label = "3) Settings",
    Key = "3" 
    }
]);

mainMenu.Run();

void DoSomething(string key)
{
    Console.WriteLine($"DoSomething - you selected, {key}!");
}