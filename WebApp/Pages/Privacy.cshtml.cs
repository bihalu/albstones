using Albstones.WebApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Albstones.WebApp.Pages;

public class PrivacyModel : PageModel
{
    public Imprint Imprint { get; set; }

    private readonly ILogger<PrivacyModel> _logger;

    private readonly IConfiguration _config;

    public PrivacyModel(ILogger<PrivacyModel> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        Imprint = _config.GetSection("Imprint").Get<Imprint>()!;
    }

    public void OnGet()
    {
    }
}
