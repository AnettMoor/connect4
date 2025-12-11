using System.ComponentModel.DataAnnotations;
using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class ContinueGame : PageModel
{
    private readonly IRepository<GameConfiguration> _configRepo;

    public ContinueGame(IRepository<GameConfiguration> configRepo)
    {
        _configRepo = configRepo;
    }
    
    public SelectList ConfigurationSelectList { get; set; } = default!;

    [BindProperty]
    public string ConfigId { get; set; } = default!;

    [BindProperty]
    [Length(3, 32)]
    public string Player1Name { get; set; } = default!;

    [BindProperty]
    [Length(3, 32)]
    public string Player2Name { get; set; } = default!;

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var data = await _configRepo.ListAsync();
        var data2 = data.Select(i => new
        {
            id = i.id,
            value = i.description
        }).ToList();
        
        ConfigurationSelectList = new SelectList(data2, "id", "value");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDataAsync();
            return Page();
        }

        // Load all configs and find the one selected
        GameConfiguration? config;

        try
        {
            config = _configRepo.Load(ConfigId);
        }
        catch
        {
            ModelState.AddModelError("", "Invalid configuration.");
            await LoadDataAsync();
            return Page();
        }

        if (config.Id == null)
        {
            ModelState.AddModelError("", "Invalid configuration.");
            await LoadDataAsync();
            return Page();
        }

        if (config.IsTemplate)
        {
            var newConfig = new GameConfiguration
            {
                Id = Guid.NewGuid(),
                Name = "Saved game: " + config.Name,
                Board = config.Board,
                BoardWidth = config.BoardWidth,
                BoardHeight = config.BoardHeight,
                WinCondition = config.WinCondition,
                IsTemplate = false
            };

            ConfigId = _configRepo.Save(newConfig);
        }

        return RedirectToPage("./GamePlay", new
        {
            id = ConfigId,
            player1Name = Player1Name,
            player2Name = Player2Name
        });
    }



}