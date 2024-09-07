using System.Globalization;
using Newtonsoft.Json;

namespace Albstones.WebApp.Models
{
    public class Albstone
    {
        public required string Address { get; set; }

        public DateTime Date { get; set; }

        public required string Name { get; set; }

        public Double Latitude { get; set; }

        public Double Longitude { get; set; }

        public required string Message { get; set; }

        public required string Image {  get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
