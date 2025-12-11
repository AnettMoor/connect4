using BLL;
using ConsoleApp;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConsoleUI;
using Newtonsoft.Json;

namespace WebApp.Pages;

public class GamePlay : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;

    public GamePlay(IRepository<GameConfiguration> configRepo)
    {
        _configRepo = configRepo;
    }

    public string GameId { get; set; } = default!;
    public GameController GameController { get; set; } = default!;


    public async Task OnGetAsync(string id, string player1Name, string player2Name, int? x,
        string? action, string? newName)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(player1Name) || string.IsNullOrEmpty(player2Name))
            throw new ArgumentException("Game ID and player names must be provided.");

        GameId = id;

        // Load the configuration from the repository
        var conf = await _configRepo.LoadAsync(id);
        if (conf == null) throw new KeyNotFoundException($"No configuration found with ID {id}");

        // Initialize GameController with the loaded board
        GameController = new GameController(conf, player1Name, player2Name);

// Restore board
        GameController.GameBrain.GameBoard = conf.Board;

// Restore turn
        GameController.GameBrain.NextMoveByX = conf.NextMoveByX;


        // Handle a move
        if (x.HasValue && !GameController.GameBrain.IsGameOver())
        {
            var result = GameController.GameBrain.TryMakeMove(x.Value);
            GameController.UpdateConfigurationBoard();
            var cfg = GameController.GetConfiguration();
            await _configRepo.UpdateAsync(cfg, id);


            // Save updated board
            GameController.UpdateConfigurationBoard();
            await _configRepo.UpdateAsync(GameController.GetConfiguration(), id);

            // DO NOT reload config here
            // DO NOT overwrite GameBoard
            // Rendering will now show updated state

            if (result.Winner != ECellState.Empty)
            {
                ViewData["Winner"] = result.Winner == ECellState.XWin ? player1Name : player2Name;
                ViewData["GameOver"] = true;
            }
        }

        // Handle save / save as new
        if (!string.IsNullOrEmpty(action) && action == "save")
        {
            if (!string.IsNullOrEmpty(newName))
            {
                var newConfig = new GameConfiguration
                {
                    Id = Guid.NewGuid(),
                    Board = GameController.GameBrain.GetBoard(),
                    Name = newName
                };
                await _configRepo.SaveAsync(newConfig);
                ViewData["Message"] = $"Game saved successfully as '{newName}'!";
            }
            else
            {
                GameController.UpdateConfigurationBoard();
                await _configRepo.UpdateAsync(GameController.GetConfiguration(),
                    GameController.GetConfiguration().Id.ToString());
                ViewData["Message"] = "Game saved successfully!";
            }
        }
    }
}
