using Albstones.Models;
using Microsoft.EntityFrameworkCore;

namespace Albstones.WebApp.Data;

public class AlbstoneDbContext : DbContext
{
    public AlbstoneDbContext(DbContextOptions<AlbstoneDbContext>options) : base(options)
    {
    }

    public DbSet<Albstone> Albstones { get; set; }
}
