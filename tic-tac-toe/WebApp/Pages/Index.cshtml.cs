using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;
    
    public IndexModel(IRepository<GameConfiguration> configRepo)
    {
        _configRepo = configRepo;
    }
    
    public List<GameConfiguration> Configurations { get; set; } = new();
    
    public async Task OnGetAsync()
    {
        // Load all saved (non-template) games
        var allItems = await _configRepo.ListAsync();

        var savedGames = new List<GameConfiguration>();

        foreach (var item in allItems)
        {
            var fullConfig = await _configRepo.LoadAsync(item.id);
            if (!fullConfig.IsTemplate)
                savedGames.Add(fullConfig);
        }

        Configurations = savedGames;
    }
}
