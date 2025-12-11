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

        // Load DB config if not in cache
        if (!GameCache.RuntimeGames.TryGetValue(id, out var runtimeConfig))
        {
            var conf = await _configRepo.LoadAsync(id);
            if (conf == null) throw new KeyNotFoundException($"No configuration found with ID {id}");

            var runtimeBoard = conf.Board != null ? CloneBoard(conf.Board) : null;

            runtimeConfig = new GameConfiguration
            {
                Id = conf.Id,
                Name = conf.Name,
                BoardWidth = conf.BoardWidth,
                BoardHeight = conf.BoardHeight,
                WinCondition = conf.WinCondition,
                IsTemplate = conf.IsTemplate,
                NextMoveByX = true,
                Board = runtimeBoard
            };

            GameCache.RuntimeGames[id] = runtimeConfig; // store in cache
        }

        // Build controller
        GameController = new GameController(runtimeConfig, player1Name, player2Name, runtimeConfig.Board);

        // Handle move
        if (x.HasValue && !GameController.GameBrain.IsGameOver())
        {
            var result = GameController.GameBrain.TryMakeMove(x.Value);

            // Update runtimeConfig (cache)
            runtimeConfig.Board = GameController.GameBrain.GetBoard();
            runtimeConfig.NextMoveByX = GameController.GameBrain.NextMoveByX;

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
                    NextMoveByX = GameController.GameBrain.NextMoveByX,
                    Name = newName
                };
                await _configRepo.SaveAsync(newConfig);
                ViewData["Message"] = $"Game saved successfully as '{newName}'!";
            }
            else
            {
                var conf = await _configRepo.LoadAsync(id);
                if (conf != null)
                {
                    conf.Board = GameController.GameBrain.GetBoard();
                    conf.NextMoveByX = GameController.GameBrain.NextMoveByX;
                    await _configRepo.UpdateAsync(conf, conf.Id.ToString());
                    ViewData["Message"] = "Game saved successfully!";
                }
            }

            // Remove runtime from cache after save
            GameCache.RuntimeGames.Remove(id);
        }
    }


    private static List<List<ECellState>> CloneBoard(List<List<ECellState>> board)
    {
        return board
            .Select(col => col.ToList())
            .ToList();
    }
}
