using System.ComponentModel.DataAnnotations;

namespace Albstones.WebApp.Models
{
    public class PageQueryParameter
    {
        [Required]
        [Range(1, 1000)]
        public int Page { get; set; } = 1;

        [Required]
        [Range(1, 9)]
        public int PageSize { get; set; } = 9;
    }
}
