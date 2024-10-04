using Albstones.Helper;
using Bogus;
using CoordinateSharp;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;

namespace Albstones.Models;

[PrimaryKey(nameof(Address), nameof(Date))]
public class Albstone
{
    public required string Address { get; set; }

    public DateTime Date { get; set; }

    public required string Name { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public required string Message { get; set; }

    public required string Image { get; set; }

    private static CultureInfo _en_US = new CultureInfo("en-US");

    public Coordinate GetCoordinate()
    {
        return new Coordinate(Latitude, Longitude, Date);
    }

    public string FormatCoordinate(bool decimalFormat = false)
    {
        if (decimalFormat)
        {
            var latitudeDecimal = Latitude.ToString("F", _en_US);
            var longitudeDecimal = Longitude.ToString("F", _en_US);

            return $"{latitudeDecimal} {longitudeDecimal}";
        }
        else
        {
            Thread.CurrentThread.CurrentCulture = _en_US;
            return GetCoordinate().ToString();
        }
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public static Albstone Parse(string json, string? image = null)
    {
        var albstone = JsonConvert.DeserializeObject<Albstone>(json)!;

        if (image != null)
        {
            var byteArray = File.ReadAllBytes(image);
            albstone.Image = Convert.ToBase64String(byteArray);
        }

        return albstone;
    }

    public static List<Albstone> FakeData(int count = 99, int randomSeed = 420, long dateTicks = 630822816000000000)
    {
        Randomizer.Seed = new Random(randomSeed);
        
        DateTime date = new DateTime(dateTicks);

        var albstoneFaker = new Faker<Albstone>("de")
            .RuleFor(a => a.Name, f => f.Name.FirstName(f.Person.Gender))
            .RuleFor(a => a.Date, f => f.Date.Future(20, date))
            .RuleFor(a => a.Latitude, f => f.Address.Latitude())
            .RuleFor(a => a.Longitude, f => f.Address.Longitude())
            .RuleFor(a => a.Message, f => f.Hacker.Phrase())
            .RuleFor(a => a.Image, DefaultImage());

        var albstones = albstoneFaker.Generate(count);

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
        using var stream = typeof(Albstone).Assembly.GetManifestResourceStream("Albstones.Images.default.png")!;
        var bytes = new Byte[(int)stream.Length];
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read(bytes, 0, (int)stream.Length);

        return Convert.ToBase64String(bytes);
    }
}
