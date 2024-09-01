using System.Text.Json;
using Albstones.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Albstones.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [ViewData]
        public List<Albstone> Albstones { get; set; }   

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        //public void OnGet()
        public async Task<IActionResult> OnGetAsync()
        {
            var baseUri = $"{Request.Scheme}://{Request.Host}";

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(baseUri + "/api/albstones?Page=1&PageSize=9"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Albstones = JsonConvert.DeserializeObject<List<Albstone>>(apiResponse);
                }
            }

            return Page();
        }
    }
}
