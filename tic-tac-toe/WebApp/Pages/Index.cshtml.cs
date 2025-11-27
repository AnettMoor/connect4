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

    public List<(string id, string description)> Configurations { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Configurations = await _configRepo.ListAsync();
    }
}