using Albstones.WebApp.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Albstones.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddRazorPages();

        var configuration = builder.Configuration;
        string database = configuration.GetValue<string>("Database") ?? "Sqlite"; // Default database Sqlite

        switch (database)
        {
            case "Sqlite":
                var sqliteConnection = new SqliteConnection(configuration.GetConnectionString(database));
                builder.Services.AddDbContext<AlbstoneDbContext>(d => d.UseSqlite(sqliteConnection));
                break;

            case "Postgres":
                var postgresConnection = new NpgsqlConnection(configuration.GetConnectionString(database));
                builder.Services.AddDbContext<AlbstoneDbContext>(d => d.UseNpgsql(postgresConnection));
                break;

            default:
                break;
        }

        builder.Services.AddScoped<AlbstoneRepository>();

        var app = builder.Build();

        AlbstoneRepository.InitializeDatabase(app);

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
