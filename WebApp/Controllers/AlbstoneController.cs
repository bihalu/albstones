using Albstones.WebApp.Data;
using Albstones.WebApp.Models;
using Albstones.Helper;
using Albstones.Models;
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
    private readonly ILogger<AlbstoneController> _logger;

    private readonly IConfiguration _config;

    private readonly AlbstoneRepository _repository;

    public AlbstoneController(ILogger<AlbstoneController> logger, IConfiguration config, AlbstoneRepository repository)
    {
        _logger = logger;
        _config = config;
        _repository = repository;
    }

    // GET: api/albstones?Page=1&PageSize=9
    [HttpGet()]
    public IActionResult Get([FromQuery] PageQueryParameter queryParameter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(_repository.GetAlbstones(queryParameter.Page, queryParameter.PageSize));
    }

    // GET api/albstones/search?Page=1&PageSize=9
    [HttpGet("{search}")]
    [DisableRequestSizeLimit]
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

        // search by address
        if (search.StartsWith("bc1"))
        {
            return Ok(_repository.GetAlbstonesByAddress(search, queryParameter.Page, queryParameter.PageSize));
        }

        // search by location
        if (Coordinate.TryParse(search, out Coordinate coordinate))
        {
            return Ok(_repository.GetAlbstonesByLocation(coordinate.Latitude.ToDouble(), coordinate.Longitude.ToDouble(), queryParameter.Page, queryParameter.PageSize));
        }

        // search by name
        return Ok(_repository.GetAlbstonesByName(search, queryParameter.Page, queryParameter.PageSize));
    }

    // POST api/albstones
    // {
    //   "Code": "YtkOnw-uyTvF21jwlnLfPC_E_btiATm97aMT3tIfwhO-D_LGwsoetO4vkk3eAzq2i9MilaHmoolrrd-yI1k4hyDUKJPapUPoM5pF0lBKxRg"
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
            // Decode and decrypt code
            var secret = _config["Secret"]!;
            var initializationVector = secret[..16].ToBytes(16);
            var key = secret[16..].ToBytes(32);

            string code = scan.Code.Replace('_', '/').Replace('-', '+');
            switch (scan.Code.Length % 4)
            {
                case 2: code += "=="; break;
                case 3: code += "="; break;
            }

            var encryptedData = Convert.FromBase64String(code);
            var decryptedData = AesEncryption.Decrypt(encryptedData, key, initializationVector);
            var words = decryptedData.BytesToString();
            var word = words.Split(' ');
            var seed = Magic.SeedHex(word);
            address = Magic.Address(seed);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }

        var albstones = _repository.GetAlbstonesByAddress(address, 1, 1);

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
        var secret = Encoding.ASCII.GetBytes(_config["Secret"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("address", address) }),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
