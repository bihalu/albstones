using Albstones.WebApp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Albstones.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        // in memory sqlite database
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddRazorPages();
        builder.Services.AddDbContext<AlbstoneDbContext>(d => d.UseSqlite(connection));
        builder.Services.AddTransient<AlbstoneRepository>();

        var app = builder.Build();

        AlbstoneRepository.AddAlbstoneData(app);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.MapRazorPages();

        app.Run();
    }
}
