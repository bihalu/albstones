using Albstones.WebApp.Models;

namespace Albstones.WebApp.Data
{
    public interface IAlbstoneRepository
    {
        IEnumerable<Albstone> GetAlbstones();

        IEnumerable<Albstone> GetAlbstonesByName(string name);

        IEnumerable<Albstone> GetAlbstonesByLocation(Double Latitude, Double Longitude);

        Albstone? GetAlbstoneByAddress(string address);
    }
}
