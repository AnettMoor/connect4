using BLL;
using ConsoleApp;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConsoleUI; // For GameController

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

    public void OnGet(string id, string player1Name, string player2Name, int? x, string? action)
    {
        GameId = id;

        // Load configuration
        var conf = _configRepo.Load(id);

        // Initialize GameController with loaded configuration
        GameController = new GameController(conf, player1Name, player2Name, conf.Board);

        // Restore whose turn it was
        if (TempData.ContainsKey("NextMoveByX"))
        {
            GameController.GameBrain.NextMoveByX = (bool)TempData["NextMoveByX"];
        }

        // Handle post-game actions (save or return)
        if (!string.IsNullOrEmpty(action))
        {
            if (action == "save")
            {
                GameController.UpdateConfigurationBoard();
                _configRepo.Update(GameController.GetConfiguration(), GameController.GetConfiguration().Id.ToString());
                ViewData["Message"] = "Game saved successfully!";
            }
            else if (action == "return")
            {
                Response.Redirect("/Index"); // Redirect to main page
            }

            return;
        }

        // Make move
        if (x.HasValue && !GameController.GameBrain.IsGameOver())
        {
            var result = GameController.GameBrain.TryMakeMove(x.Value);

            // Store updated turn
            TempData["NextMoveByX"] = GameController.GameBrain.NextMoveByX;

            // Update board in configuration
            GameController.UpdateConfigurationBoard();
            _configRepo.Update(GameController.GetConfiguration(), GameController.GetConfiguration().Id.ToString());

            // Check winner or draw
            if (result.Winner != ECellState.Empty)
            {
                ViewData["Winner"] = result.Winner == ECellState.XWin ? player1Name : player2Name;
                ViewData["GameOver"] = true; // Flag for Razor page to show save/return options
            
            }
        }
    }

}