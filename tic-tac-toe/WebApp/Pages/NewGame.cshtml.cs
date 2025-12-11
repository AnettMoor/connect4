using System.ComponentModel.DataAnnotations;
using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class NewGame : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;

    public NewGame(IRepository<GameConfiguration> configRepo)
    {
        _configRepo = configRepo;
    }

    public SelectList ConfigurationSelectList { get; set; } = default!;

    [BindProperty] public string ConfigId { get; set; } = default!;

    [BindProperty] [Length(3, 32)] public string Player1Name { get; set; } = default!;

    [BindProperty] [Length(3, 32)] public string Player2Name { get; set; } = default!;

    // fir custom configurations
    [BindProperty] public bool IsCustom { get; set; } = false;

    [BindProperty]
    public int BoardWidth { get; set; } = 5;

    [BindProperty]
    public int BoardHeight { get; set; } = 5;

    [BindProperty]
    public int WinCondition { get; set; } = 4;

    public async Task OnGetAsync()
    {
        await LoadTemplatesAsync();
    }

    private async Task LoadTemplatesAsync()
    {
        // Load all items (id + description)
        var allItems = await _configRepo.ListAsync();

        var templates = new List<GameConfiguration>();

        // Load full configuration for each item asynchronously
        foreach (var item in allItems)
        {
            var fullConfig = await _configRepo.LoadAsync(item.id);
            if (fullConfig.IsTemplate)
                templates.Add(fullConfig);
        }

        // configuration list
        var selectItems = templates
            .Select(i => new { id = i.Id, value = i.Name })
            .ToList();
        
        ConfigurationSelectList = new SelectList(selectItems, "id", "value");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadTemplatesAsync();
            return Page();
        }
        
        string configIdToUse = ConfigId;

        // If custom, generate a new configuration
        if (IsCustom)
        {
            var newConfig = new GameConfiguration
            {
                Id = Guid.NewGuid(),
                Name = "Custom",
                BoardWidth = BoardWidth,
                BoardHeight = BoardHeight,
                WinCondition = WinCondition,
                IsTemplate = false,
            };

            await _configRepo.SaveAsync(newConfig);
            configIdToUse = newConfig.Id.ToString();
        }
        else if (ConfigId == "custom" || string.IsNullOrEmpty(ConfigId))
        {
            // Invalid template selection for customs
            ModelState.AddModelError("ConfigId", "Please reselect template.");
            await LoadTemplatesAsync();
            return Page();
        }

        return RedirectToPage("./GamePlay", new
        {
            id = configIdToUse,
            player1Name = Player1Name,
            player2Name = Player2Name
        });
    }
}