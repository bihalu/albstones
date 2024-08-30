using Albstones.WebApp.Data;
using Albstones.WebApp.Models;
using CoordinateSharp;
using Microsoft.AspNetCore.Mvc;

namespace Albstones.WebApp.Controllers
{
    [Route("api/albstones")]
    [ApiController]
    public class AlbstoneController : ControllerBase
    {
        private AlbstoneRepository repository = new AlbstoneRepository();

        // GET: api/albstones?page=1&page_size=9
        [HttpGet()]
        public IEnumerable<Albstone> Get([FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "page_size")] int pageSize = 9)
        {
            return repository.GetAlbstones();
        }

        // GET api/albstones/search?page=1&page_size=9
        [HttpGet("{search}")]
        public IEnumerable<Albstone> Get(string search, [FromQuery(Name = "page")] int page = 1, [FromQuery(Name = "page_size")] int pageSize = 9)
        {
            var albstones = new List<Albstone>();

            // search nothing
            if (string.IsNullOrWhiteSpace(search))
            {
                return albstones;
            }

            // search for address
            if (search.StartsWith("bc"))
            {
                var albstone = repository.GetAlbstoneByAddress(search);
                if (albstone == null)
                {
                    return albstones;
                }
                else
                {
                    albstones.Add(albstone);
                    return albstones;
                }
            }

            // search for location
            if (Coordinate.TryParse(search, out Coordinate coordinate))
            {
                return repository.GetAlbstonesByLocation(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble());
            }

            // search for name
            return repository.GetAlbstonesByName(search);
        }
    }
}
