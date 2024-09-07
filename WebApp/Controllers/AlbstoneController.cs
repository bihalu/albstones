using Albstones.Models;
using Albstones.WebApp.Data;
using Albstones.WebApp.Models;
using CoordinateSharp;
using Microsoft.AspNetCore.Mvc;

namespace Albstones.WebApp.Controllers;

[Route("api/albstones")]
[ApiController]
public class AlbstoneController : ControllerBase
{
    private AlbstoneRepository repository;

    public AlbstoneController(AlbstoneRepository repository)
    {
        this.repository = repository;
    }

    // GET: api/albstones?Page=1&PageSize=9
    [HttpGet()]
    public IActionResult Get([FromQuery] PageQueryParameter queryParameter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(repository.GetAlbstones(queryParameter.Page, queryParameter.PageSize));
    }

    // GET api/albstones/search?Page=1&PageSize=9
    [HttpGet("{search}")]
    public IActionResult Get(string search, [FromQuery] PageQueryParameter queryParameter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var albstones = new List<Albstone>();

        // search nothing
        if (string.IsNullOrWhiteSpace(search))
        {
            return Ok(albstones);
        }

        // search for address
        if (search.StartsWith("bc"))
        {
            return Ok(repository.GetAlbstonesByAddress(search, queryParameter.Page, queryParameter.PageSize));
        }

        // search for location
        if (Coordinate.TryParse(search, out Coordinate coordinate))
        {
            return Ok(repository.GetAlbstonesByLocation(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), queryParameter.Page, queryParameter.PageSize));
        }

        // search for name
        return Ok(repository.GetAlbstonesByName(search, queryParameter.Page, queryParameter.PageSize));
    }
}
