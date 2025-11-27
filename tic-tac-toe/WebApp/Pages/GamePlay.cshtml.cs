using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class GamePlay : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;

    public GamePlay(IRepository<GameConfiguration> configRepo)
    {
        _configRepo = configRepo;
    }

    public string GameId { get; set; } = default!;
    public GameBrain GameBrain { get; set; } = default!;
    
    public void OnGet(string id, string player1Name, string player2Name, int? x, int? y)
    {
        GameId = id;
        
        var conf = _configRepo.Load(id);
        GameBrain = new GameBrain(conf, player1Name, player2Name);
        // restore state
        if (x.HasValue && y.HasValue)
        {
            GameBrain.ProcessMove(x.Value, y.Value);
            // save state
        }
    }
}