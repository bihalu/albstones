using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Albstones.WebApp.Pages;

public class TutorialModel : PageModel
{
    private readonly ILogger<TutorialModel> _logger;

    private readonly IConfiguration _config;

    public TutorialModel(ILogger<TutorialModel> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }
    public void OnGet()
    {
    }
}
