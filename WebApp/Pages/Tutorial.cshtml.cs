﻿using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Albstones.WebApp.Pages;

public class TutorialModel : PageModel
{
    private readonly ILogger<TutorialModel> _logger;

    public TutorialModel(ILogger<TutorialModel> logger)
    {
        _logger = logger;
    }
    public void OnGet()
    {
    }
}
