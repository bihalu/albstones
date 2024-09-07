using System.ComponentModel.DataAnnotations;

namespace Albstones.WebApp.Models;

public class PageQueryParameter
{
    [Range(1, 1000)]
    public required int Page { get; set; } = 1;

    [Range(1, 9)]
    public required int PageSize { get; set; } = 9;
}
