using Albstones.Helpers;
using Albstones.Models;
using Albstones.WebApp.Data;
using Albstones.WebApp.Models;
using CoordinateSharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Albstones.WebApp.Controllers;

[Route("api/albstones")]
[ApiController]
public class AlbstoneController : ControllerBase
{
    private readonly AlbstoneRepository Repository;

    private readonly IConfiguration Configuration;

    public AlbstoneController(AlbstoneRepository repository, IConfiguration configuration)
    {
        Repository = repository;
        Configuration = configuration;
    }

    // GET: api/albstones?Page=1&PageSize=9
    [HttpGet()]
    public IActionResult Get([FromQuery] PageQueryParameter queryParameter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(Repository.GetAlbstones(queryParameter.Page, queryParameter.PageSize));
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
        if (search.StartsWith("bc1"))
        {
            return Ok(Repository.GetAlbstonesByAddress(search, queryParameter.Page, queryParameter.PageSize));
        }

        // search for location
        if (Coordinate.TryParse(search, out Coordinate coordinate))
        {
            return Ok(Repository.GetAlbstonesByLocation(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), queryParameter.Page, queryParameter.PageSize));
        }

        // search for name
        return Ok(Repository.GetAlbstonesByName(search, queryParameter.Page, queryParameter.PageSize));
    }

    // POST api/albstones
    // {
    //   "Scan": "amFja2V0IG9hayBoYWJpdCBuYWl2ZSBuYWl2ZSB5YXJkIGFiYW5kb24gbGFiIGJhYnkgc2FkIHRhYmxlIG9mdGVu"
    // }
    [HttpPost()]
    public IActionResult Post(ScanParameter scan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var address = "";

        try
        {
            var seed = Magic.SeedHex(scan.GetWords());
            address = Magic.Address(seed);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }

        var albstones = Repository.GetAlbstonesByAddress(address, 1, 1);

        if (albstones == null || albstones.Count() != 1)
        {
            return NotFound();
        }

        return Ok(GetToken(address));
    }

    // JWT
    private string GetToken(string address)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Configuration["Secret"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("address", address) }),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
