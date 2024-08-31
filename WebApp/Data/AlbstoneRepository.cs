﻿using Albstones.WebApp.Helpers;
using Albstones.WebApp.Models;
using Bogus;
using CoordinateSharp;
using Microsoft.EntityFrameworkCore;

namespace Albstones.WebApp.Data
{
    public class AlbstoneRepository : IAlbstoneRepository
    {
        public readonly AlbstoneDbContext context;

        public AlbstoneRepository(AlbstoneDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Albstone> GetAlbstonesByAddress(string address)
        {
            return context.Albstones.Where(a => a.Address == address);
        }

        public IEnumerable<Albstone> GetAlbstones()
        {
            // get only the first albstone for each address, use good old SQL ;-)
            return context.Albstones.FromSql($"SELECT Address, Name, MIN(Date) AS Date, Latitude, Longitude, Message, Image FROM Albstones GROUP BY Address, Name, Latitude, Longitude, Message, Image HAVING Date = MIN(Date)").ToList();
        }

        public IEnumerable<Albstone> GetAlbstonesByLocation(double Latitude, double Longitude, int radius = 1000)
        {
            Coordinate coordinate1 = new Coordinate(Latitude, Longitude);

            foreach (var albstone in context.Albstones)
            {
                Coordinate coordinate2 = new Coordinate(albstone.Latitude, albstone.Longitude);
                Distance distance = new Distance(coordinate1, coordinate2);

                if (distance.Meters < radius)
                {
                    yield return albstone;
                }
            }
        }

        public IEnumerable<Albstone> GetAlbstonesByName(string name)
        {
            return context.Albstones.Where(a => a.Name == name);
        }

        public static void AddAlbstoneData(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<AlbstoneDbContext>();

            context!.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            foreach (var albstone in AlbstoneRepository.FakeData())
            {
                context.Albstones.Add(albstone);
            }

            context.SaveChanges();
        }

        private static List<Albstone> FakeData()
        {
            Randomizer.Seed = new Random(420);
            DateTime millenium = new DateTime(2000, 1, 1, 0, 0, 0);

            var albstoneFaker = new Faker<Albstone>("de")
                .RuleFor(a => a.Name, f => f.Name.FirstName(f.Person.Gender))
                .RuleFor(a => a.Date, f => f.Date.Future(20, millenium))
                .RuleFor(a => a.Latitude, f => f.Address.Latitude())
                .RuleFor(a => a.Longitude, f => f.Address.Longitude())
                .RuleFor(a => a.Message, f => f.Hacker.Phrase())
                .RuleFor(a => a.Image, Albstone.DefaultImage());

            var albstones = albstoneFaker.Generate(99); // generate 99 albstones

            foreach (var albstone in albstones)
            {
                Coordinate coordinate = new Coordinate(albstone.Latitude, albstone.Longitude, albstone.Date);
                string[] mnemonic = Magic.Mnemonic(albstone.Name, coordinate);
                string seed = Magic.SeedHex(mnemonic);
                albstone.Address = Magic.Address(seed, 0);
            }

            return albstones;
        }
    }
}
