using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
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
    
    // for delete success message
    [TempData]
    public string? StatusMessage { get; set; } 
    
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
    private async Task LoadConfigurationsAsync()
    {
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
    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            var config = await _configRepo.LoadAsync(id);
            if (config != null)
            {
                await _configRepo.DeleteAsync(id);
                StatusMessage = $"Game '{config.Name}' deleted successfully!";
            }
        }

        await LoadConfigurationsAsync(); // Refresh list
        return Page();
    }
}
