using Albstones.Models;
using CoordinateSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Albstones.WebApp.Data;

public class AlbstoneRepository : IAlbstoneRepository
{
    private readonly ILogger<AlbstoneRepository> _logger;

    private readonly AlbstoneDbContext _context;

    private readonly IConfiguration _config;

    public AlbstoneRepository(ILogger<AlbstoneRepository> logger, AlbstoneDbContext context, IConfiguration config)
    {
        _logger = logger;
        _context = context;
        _config = config;
    }

    public IEnumerable<Albstone> GetAlbstonesByAddress(string address, int page, int pageSize)
    {
        _logger.LogInformation("GetAlbstonesByAddress Address={address} Page={page} PageSize={pageSize}", address, page, pageSize);

        var query = _context.Albstones.Where(a => a.Address == address).OrderByDescending(s => s.Date);

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public IEnumerable<Albstone> GetAlbstones(int page, int pageSize)
    {
        _logger.LogInformation("GetAlbstones Page={page} PageSize={pageSize}", page, pageSize);

        string database = _config.GetValue<string>("DATABASE") ?? "Sqlite";

        IQueryable<Albstone> query;

        switch (database)
        {
            case "Sqlite":
                query = _context.Albstones.FromSql($"SELECT Address, Name, MIN(Date) AS Date, Latitude, Longitude, Message, Image FROM Albstones GROUP BY Address, Name, Latitude, Longitude, Message, Image HAVING Date = MIN(Date)");
                break;

            case "Postgres":
                query = _context.Albstones.FromSql($"SELECT a.\"Address\", a.\"Name\", a.\"Date\", a.\"Latitude\", a.\"Longitude\", a.\"Message\", a.\"Image\" FROM public.\"Albstones\" a JOIN(SELECT \"Address\", MIN(\"Date\") AS \"MinDate\" FROM public.\"Albstones\" GROUP BY \"Address\") AS j ON a.\"Address\" = j.\"Address\" AND a.\"Date\" = j.\"MinDate\"");
                break;

            default:
                query = _context.Albstones;
                break;
        }

        return query.Skip((page - 1) * pageSize).Take(pageSize);
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

        return albstones.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public IEnumerable<Albstone> GetAlbstonesByName(string name, int page, int pageSize)
    {
        _logger.LogInformation("GetAlbstonesByName Name={name} Page={page} PageSize={pageSize}", name, page, pageSize);

        var query = _context.Albstones.Where(a => a.Name == name).OrderByDescending(s => s.Date);

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public static void InitializeAlbstoneDatabase(WebApplication app)
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
