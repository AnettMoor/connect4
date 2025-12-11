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

        // Prepare select list
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

        // Redirect to gameplay page with selected template
        return RedirectToPage("./GamePlay", new
        {
            id = ConfigId,
            player1Name = Player1Name,
            player2Name = Player2Name
        });
    }
}
