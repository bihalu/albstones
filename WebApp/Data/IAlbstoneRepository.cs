using Albstones.Models;

namespace Albstones.WebApp.Data;

public interface IAlbstoneRepository
{
    IEnumerable<Albstone> GetAlbstones(int page, int pageSize);

    IEnumerable<Albstone> GetAlbstonesByName(string name, int page, int pageSize);

    IEnumerable<Albstone> GetAlbstonesByLocation(Double Latitude, Double Longitude, int page, int pageSize, int radius);

    IEnumerable<Albstone> GetAlbstonesByAddress(string address, int page, int pageSize);
}
