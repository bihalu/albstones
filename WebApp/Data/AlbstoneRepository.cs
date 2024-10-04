using Albstones.Models;
using CoordinateSharp;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Albstones.WebApp.Data;

public class AlbstoneRepository : IAlbstoneRepository
{
    private readonly ILogger<AlbstoneRepository> _logger;

    public readonly AlbstoneDbContext context;

    public AlbstoneRepository(ILogger<AlbstoneRepository> logger, AlbstoneDbContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public IEnumerable<Albstone> GetAlbstonesByAddress(string address, int page, int pageSize)
    {
        var query = context.Albstones.Where(a => a.Address == address).OrderByDescending(s => s.Date);

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public IEnumerable<Albstone> GetAlbstones(int page, int pageSize)
    {
        // get only the first albstone for each address, use good old SQL ;-)
        var query = context.Albstones.FromSql($"SELECT Address, Name, MIN(Date) AS Date, Latitude, Longitude, Message, Image FROM Albstones GROUP BY Address, Name, Latitude, Longitude, Message, Image HAVING Date = MIN(Date)");

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public IEnumerable<Albstone> GetAlbstonesByLocation(double Latitude, double Longitude, int page, int pageSize, int radius = 1000)
    {
        List<Albstone> albstones = new();
        Coordinate coordinateA = new Coordinate(Latitude, Longitude);

        foreach (var albstone in context.Albstones)
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
        var query = context.Albstones.Where(a => a.Name == name).OrderByDescending(s => s.Date);

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public static void AddAlbstoneData(WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<AlbstoneDbContext>();

        context!.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        if (File.Exists("albstones.json"))
        {
            try
            {
                var json = File.ReadAllText("albstones.json");
                var albstones = JsonConvert.DeserializeObject<List<Albstone>>(json);

                context.Albstones.AddRange(albstones!);

                context.SaveChanges();

                return;
            }
            catch (Exception)
            {
                // Can't read albstones.json, fallback to fake data
            }
        }

        // Use fake data
        foreach (var albstone in Albstone.FakeData())
        {
            context.Albstones.Add(albstone);
        }

        context.SaveChanges();
    }
}
