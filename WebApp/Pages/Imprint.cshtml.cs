using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Albstones.WebApp.Pages;

public class ImprintModel : PageModel
{
    private readonly ILogger<ImprintModel> _logger;

    public ImprintModel(ILogger<ImprintModel> logger)
    {
        _logger = logger;     
    }

    public void OnGet()
    {
    }
}
