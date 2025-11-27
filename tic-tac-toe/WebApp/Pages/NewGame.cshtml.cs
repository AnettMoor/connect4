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
        
        // TODO: save game
        
        // redirect to gameplay
        return RedirectToPage("./GamePlay", new { id = ConfigId, player1Name = Player1Name, player2Name = Player2Name });
    }

}