using Albstones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Albstones.WebApp.Pages;

public class SearchModel : PageModel
{
    public List<Albstone> Albstones { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Item {  get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNr { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 9;

    public int Previous { get { return PageNr > 1 ? PageNr - 1 : 1; } }
    
    public int Next { get { return Albstones.Count == PageSize ? PageNr + 1 : PageNr; } }

    private readonly ILogger<SearchModel> _logger;

    private readonly IConfiguration _config;

    public SearchModel(ILogger<SearchModel> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        Albstones = new List<Albstone>();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var baseUri = $"{Request.Scheme}://{Request.Host}";

        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync($"{baseUri}/api/albstones/{Item}?Page={PageNr}&PageSize={PageSize}");
        var apiResponse = await response.Content.ReadAsStringAsync();
#pragma warning disable CS8601 // Mögliche Nullverweiszuweisung.
        Albstones = JsonConvert.DeserializeObject<List<Albstone>>(apiResponse);
#pragma warning restore CS8601 // Mögliche Nullverweiszuweisung.

        return Page();
    }
}
