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
}
