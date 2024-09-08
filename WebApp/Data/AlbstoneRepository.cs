using Bogus;
using Albstones.Helper;
using Albstones.Models;
using CoordinateSharp;
using Microsoft.EntityFrameworkCore;

namespace Albstones.WebApp.Data;

public class AlbstoneRepository : IAlbstoneRepository
{
    public readonly AlbstoneDbContext context;

    public AlbstoneRepository(AlbstoneDbContext context)
    {
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
        Coordinate coordinate1 = new Coordinate(Latitude, Longitude);

        foreach (var albstone in context.Albstones)
        {
            Coordinate coordinate2 = new Coordinate(albstone.Latitude, albstone.Longitude);
            Distance distance = new Distance(coordinate1, coordinate2);

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

        foreach (var albstone in AlbstoneRepository.FakeData())
        {
            context.Albstones.Add(albstone);
        }

        context.SaveChanges();
    }

    private static List<Albstone> FakeData()
    {
        Randomizer.Seed = new Random(420);
        DateTime millenium = new DateTime(2000, 1, 1, 0, 0, 0);

        var albstoneFaker = new Faker<Albstone>("de")
            .RuleFor(a => a.Name, f => f.Name.FirstName(f.Person.Gender))
            .RuleFor(a => a.Date, f => f.Date.Future(20, millenium))
            .RuleFor(a => a.Latitude, f => f.Address.Latitude())
            .RuleFor(a => a.Longitude, f => f.Address.Longitude())
            .RuleFor(a => a.Message, f => f.Hacker.Phrase())
            .RuleFor(a => a.Image, DefaultImage());

        var albstones = albstoneFaker.Generate(99); // generate 99 albstones

        foreach (var albstone in albstones)
        {
            Coordinate coordinate = new Coordinate(albstone.Latitude, albstone.Longitude, albstone.Date);
            string[] mnemonic = Magic.Mnemonic(albstone.Name, coordinate);
            string seed = Magic.SeedHex(mnemonic);
            albstone.Address = Magic.Address(seed, 0);
        }

        return albstones;
    }

    public static string DefaultImage()
    {
        using var stream = typeof(Program).Assembly.GetManifestResourceStream("Albstones.WebApp.wwwroot.default.png")!;
        var bytes = new Byte[(int)stream.Length];
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read(bytes, 0, (int)stream.Length);
        return Convert.ToBase64String(bytes);
    }
}
