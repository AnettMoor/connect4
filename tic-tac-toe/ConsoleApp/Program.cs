// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design;
using MenuSystem;

Console.WriteLine("Hello, TIC-TAC-TOE");

var mainMenu = new Menu();

mainMenu.AddMenuItems(
[
    new MenuItem
    {
        Label = "Label 1",
        Key = "1"
    },
    new MenuItem
    {
        Label = "Label 2",
        Key = "2" 
    } 
]);

mainMenu.Run();