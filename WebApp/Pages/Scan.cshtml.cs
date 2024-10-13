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
            var json = new StringContent($"{{ \"Code\": \"{Code}\" }}", Encoding.UTF8, "application/json");

            var baseUrl = System.Environment.GetEnvironmentVariable("BASE_URL") ?? $"{Request.Scheme}://{Request.Host}";

            using var httpClient = new HttpClient();
            using var response = await httpClient.PostAsync($"{baseUrl}/api/albstones", json);
            var apiResponse = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                FoundAlbstone = true;
            }
        }

        return Page();
    }
}
