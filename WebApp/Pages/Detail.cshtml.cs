using Albstones.Helper;
using Albstones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Albstones.WebApp.Pages;

public class DetailModel : PageModel
{
    public List<Albstone> Albstones { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Item { get; set; }

    private readonly ILogger<DetailModel> _logger;

    private readonly IConfiguration _config;

    public DetailModel(ILogger<DetailModel> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        Albstones = new List<Albstone>();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var baseUrl = System.Environment.GetEnvironmentVariable("BASE_URL") ?? $"{Request.Scheme}://{Request.Host}";

        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync($"{baseUrl}/api/albstones/{Item}?Page=1&PageSize=9");
        var apiResponse = await response.Content.ReadAsStringAsync();
        Albstones = JsonConvert.DeserializeObject<List<Albstone>>(apiResponse)!;

        return Page();
    }
}
