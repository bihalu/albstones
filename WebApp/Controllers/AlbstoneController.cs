using Albstones.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Albstones.WebApp.Controllers
{
    [Route("api/albstones")]
    [ApiController]
    public class AlbstoneController : ControllerBase
    {
        // GET: api/albstones?page=1&page_size=9
        [HttpGet()]
        public /*IActionResult*/ IEnumerable<Albstone> Get([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "page_size")] int pageSize = 9)
        {
            var albstones = new List<Albstone>()
            {
                {
                    new Albstone()
                    {
                    Address = "Address1",
                    Name = "Name1",
                    Date = new DateTime(1972, 01, 09, 08, 00, 00),
                    Latitude = 48.3553639,
                    Longitude = 8.9680407,
                    Message = "Message1",
                    Image = "Image1"
                    }
                },
                {
                    new Albstone()
                    {
                    Address = "Address2",
                    Name = "Name2",
                    Date = new DateTime(1972, 01, 09, 08, 00, 00),
                    Latitude = 48.3553639,
                    Longitude = 8.9680407,
                    Message = "Message2",
                    Image = "Image2"
                    }
                }
            };

            return albstones;
        }

        // GET api/albstones/hugo?page=1&page_size=9
        [HttpGet("{search}")]
        public /*IActionResult*/ IEnumerable<Albstone> Get(string search, [FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "page_size")] int pageSize = 9)
        {

            // search start with bc => search for address

            // search starts with digit => search for location

            // search starts with character => search for name

            // search starts with * => search for name like

            var albstones = new List<Albstone>()
            {
                {
                    new Albstone()
                    {
                    Address = "Address1",
                    Name = "Name1",
                    Date = new DateTime(1972, 01, 09, 08, 00, 00),
                    Latitude = 48.3553639,
                    Longitude = 8.9680407,
                    Message = "Message1",
                    Image = "Image1"
                    }
                },
                {
                    new Albstone()
                    {
                    Address = "Address2",
                    Name = "Name2",
                    Date = new DateTime(1972, 01, 09, 08, 00, 00),
                    Latitude = 48.3553639,
                    Longitude = 8.9680407,
                    Message = "Message2",
                    Image = "Image2"
                    }
                }
            };

            return albstones;
        }
    }
}
