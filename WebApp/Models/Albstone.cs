using CoordinateSharp;

namespace Albstones.WebApp.Models
{
    public class Albstone
    {
        public string Fingerprint { get; set; }

        public string Name { get; set; }

        public Coordinate Coordinate { get; set; }

        public DateTime Date { get; set; }
    }
}
