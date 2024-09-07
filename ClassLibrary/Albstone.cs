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

    public Double Latitude { get; set; }

    public Double Longitude { get; set; }

    public required string Message { get; set; }

    public required string Image {  get; set; }

    public Coordinate GetCoordinate()
    {
        return new Coordinate(Latitude, Longitude, Date);
    }

    public string FormatCoordinate(bool decimalFormat = false)
    {
        if (decimalFormat)
        {
            string latitudeDecimal = Latitude.ToString("F", CultureInfo.CreateSpecificCulture("en-US"));
            string longitudeDecimal = Longitude.ToString("F", CultureInfo.CreateSpecificCulture("en-US"));

            return $"{latitudeDecimal} {longitudeDecimal}";
        }
        else
        {
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
            byte[] byteArray = File.ReadAllBytes(image);
            albstone.Image = Convert.ToBase64String(byteArray);
        }

        return albstone;
    }
}
