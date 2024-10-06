using Albstones.Models;
using CoordinateSharp;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Albstones.WebApp.Data;

public class AlbstoneRepository : IAlbstoneRepository
{
    private readonly ILogger<AlbstoneRepository> _logger;

    private readonly IConfiguration _config;

    private readonly AlbstoneDbContext _context;

    public AlbstoneRepository(ILogger<AlbstoneRepository> logger, IConfiguration config, AlbstoneDbContext context)
    {
        _logger = logger;
        _config = config;
        _context = context;
    }

    public IEnumerable<Albstone> GetAlbstonesByAddress(string address, int page, int pageSize)
    {
        _logger.LogInformation("GetAlbstonesByAddress Address={address} Page={page} PageSize={pageSize}", address, page, pageSize);

        var query = _context.Albstones.Where(a => a.Address == address);

        return query.OrderByDescending(a => a.Date).Skip((page - 1) * pageSize).Take(pageSize);
    }

    public IEnumerable<Albstone> GetAlbstones(int page, int pageSize)
    {
        _logger.LogInformation("GetAlbstones Page={page} PageSize={pageSize}", page, pageSize);

        var query = _context.Albstones.Join(
            _context.Albstones
            .GroupBy(a => a.Address)
            .Select(a => new
            {
                Address = a.Key,
                MinDate = a.Min(a => a.Date),
            }),
            a => a.Address,
            b => b.Address,
            (a, b) => new
            {
                a.Address,
                a.Date,
                a.Name,
                a.Latitude,
                a.Longitude,
                a.Message,
                a.Image,
                FirstAddress = b.Address,
                b.MinDate,
            }
        )
        .Where(a => a.Date == a.MinDate && a.Address == a.FirstAddress)
        .Select(a => new Albstone()
        {
            Address = a.Address,
            Date = a.Date,
            Name = a.Name,
            Latitude = a.Latitude,
            Longitude = a.Longitude,
            Message = a.Message,
            Image = a.Image,
        });

        return query.OrderBy(a => a.Address).Skip((page - 1) * pageSize).Take(pageSize);
    }

    public IEnumerable<Albstone> GetAlbstonesByLocation(double latitude, double longitude, int page, int pageSize, int radius = 1000)
    {
        _logger.LogInformation("GetAlbstonesByLocation Latitude={latitude} Longitude={longitude} Radius={radius} Page={page} PageSize={pageSize}", latitude, longitude, radius, page, pageSize);

        List<Albstone> albstones = new();
        Coordinate coordinateA = new Coordinate(latitude, longitude);

        foreach (var albstone in _context.Albstones)
        {
            Coordinate coordinateB = new Coordinate(albstone.Latitude, albstone.Longitude);
            Distance distance = new Distance(coordinateA, coordinateB);

            if (distance.Meters < radius)
            {
                albstones.Add(albstone);
            }
        }

        return albstones.OrderBy(a => a.Address).Skip((page - 1) * pageSize).Take(pageSize);
    }

    public IEnumerable<Albstone> GetAlbstonesByName(string name, int page, int pageSize)
    {
        _logger.LogInformation("GetAlbstonesByName Name={name} Page={page} PageSize={pageSize}", name, page, pageSize);

        var query = _context.Albstones.Where(a => a.Name == name);

        return query.OrderByDescending(a => a.Date).Skip((page - 1) * pageSize).Take(pageSize);
    }

    public static void InitializeDatabase(WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<AlbstoneDbContext>();
        var logger = app.Services.GetService<ILogger<AlbstoneRepository>>();

        context!.Database.OpenConnection();

        if (context.Database.EnsureCreated())
        {
            if (File.Exists("albstones.json"))
            {
                try
                {
                    var json = File.ReadAllText("albstones.json");
                    var albstones = JsonConvert.DeserializeObject<List<Albstone>>(json);

                    context.Albstones.AddRange(albstones!);

                    logger!.LogInformation("Add {count} albstones from albstones.json", albstones!.Count);

                    context.SaveChanges();

                    return;
                }
                catch (Exception exception)
                {
                    // Can't read albstones.json, fallback to fake data
                    logger!.LogWarning("Error reading albstones.json data, {message}", exception.Message);
                }
            }

            // Use fake data
            foreach (var albstone in Albstone.FakeData())
            {
                context.Albstones.Add(albstone);
            }

            logger!.LogInformation("Add {count} fake albstones", Albstone.FakeData().Count);

            context.SaveChanges();
        }
    }
}
