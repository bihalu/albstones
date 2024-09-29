using Albstones.WebApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Albstones.WebApp.Pages;

public class ImprintModel : PageModel
{
    public Imprint Imprint { get; set; }

    private readonly ILogger<ImprintModel> _logger;

    private readonly IConfiguration _config;

    public ImprintModel(ILogger<ImprintModel> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        Imprint = _config.GetSection("Imprint").Get<Imprint>()!;
    }

    public void OnGet()
    {
    }
}
