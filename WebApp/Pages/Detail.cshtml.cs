using Albstones.Helpers;
using Albstones.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Albstones.WebApp.Pages;

public class DetailModel : PageModel
{
    private readonly ILogger<DetailModel> _logger;

    [ViewData]
    public List<Albstone> Albstones { get; set; }

    [ViewData]
    public string Words { get; set; } = "";

    [ViewData]
    public string WordsBase64 { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string? Item { get; set; }

    public DetailModel(ILogger<DetailModel> logger)
    {
        _logger = logger;
        Albstones = new List<Albstone>();
    }
    public async Task<IActionResult> OnGetAsync()
    {
        var baseUri = $"{Request.Scheme}://{Request.Host}";

        using (var httpClient = new HttpClient())
        {
            using (var response = await httpClient.GetAsync($"{baseUri}/api/albstones/{Item}?Page=1&PageSize=9"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                Albstones = JsonConvert.DeserializeObject<List<Albstone>>(apiResponse)!;

                Words = string.Join(' ', Magic.Mnemonic(Albstones[0].Name, Albstones[0].GetCoordinate()));

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Words);
                WordsBase64 = Convert.ToBase64String(plainTextBytes);
            }
        }

        return Page();
    }
}
