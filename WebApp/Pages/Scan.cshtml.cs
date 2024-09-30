using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace Albstones.WebApp.Pages;

public class ScanModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Code { get; set; }

    public bool FoundAlbstone { get; set; }

    private readonly ILogger<ScanModel> _logger;

    private readonly IConfiguration _config;

    public ScanModel(ILogger<ScanModel> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!string.IsNullOrEmpty(Code))
        {
            var json = new StringContent($"{{ \"Scan\": \"{Code}\" }}", Encoding.UTF8, "application/json");

            var baseUri = $"{Request.Scheme}://{Request.Host}";

            using var httpClient = new HttpClient();
            using var response = await httpClient.PostAsync($"{baseUri}/api/albstones", json);
            var apiResponse = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                FoundAlbstone = true;
            }
        }

        return Page();
    }
}
