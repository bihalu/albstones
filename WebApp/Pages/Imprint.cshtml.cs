using Albstones.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Albstones.WebApp.Pages;

public class ImprintModel : PageModel
{
    private readonly ILogger<ImprintModel> _logger;

    private readonly IConfiguration Configuration;

    [ViewData]
    public Imprint Imprint { get; set; }

    public ImprintModel(ILogger<ImprintModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        Configuration = configuration;
        Imprint = Configuration.GetSection("Imprint").Get<Imprint>()!;
    }

    public void OnGet()
    {
    }
}
