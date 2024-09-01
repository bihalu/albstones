using System.Globalization;
using CoordinateSharp;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Albstones.WebApp.Models
{
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

        public string FormatCoordinate(bool dec = false)
        {
            if (dec)
            {
                return $"{Latitude.ToString("F", CultureInfo.CreateSpecificCulture("en-US"))} {Longitude.ToString("F", CultureInfo.CreateSpecificCulture("en-US"))}";
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

        public static string DefaultImage()
        {
            using var stream = typeof(Program).Assembly.GetManifestResourceStream("Albstones.WebApp.wwwroot.default.png")!;
            var bytes = new Byte[(int)stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, (int)stream.Length);
            return Convert.ToBase64String(bytes);
        }
    }
}
