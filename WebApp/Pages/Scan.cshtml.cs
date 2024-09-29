using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Albstones.WebApp.Pages;

public class ScanModel : PageModel
{
    private readonly ILogger<ScanModel> _logger;

    private readonly IConfiguration _config;

    public ScanModel(ILogger<ScanModel> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public void OnGet()
    {
    }
}
