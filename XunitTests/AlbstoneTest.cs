﻿using Albstones.Helper;
using Albstones.Models;
using Albstones.WebApp.Data;
using Bogus;
using CoordinateSharp;
using Serilog;
using Xunit.Abstractions;

namespace XunitTests;

public class AlbstoneTest
{
    public AlbstoneTest(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.TestOutput(output).CreateLogger();
    }

    [Fact]
    public void AlbstoneToJson()
    {
        // Arrange
        var expected = File.ReadAllText("Testdata/albstone.json");
        var albstone = new Albstone()
        {
            Address = "Address",
            Name = "Name",
            Date = new DateTime(1972, 01, 09, 08, 00, 00),
            Latitude = 48.3553639,
            Longitude = 8.9680407,
            Message = "Message",
            Image = "Image",
        };

        // Act
        var actual = albstone.ToJson();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AlbstoneFormatCoordinate()
    {
        // Arrange
        var expected = @"N 48º 21' 19.31"" E 8º 58' 4.947""";
        var albstone = new Albstone()
        {
            Address = "Address",
            Name = "Name",
            Date = new DateTime(1972, 01, 09, 08, 00, 00),
            Latitude = 48.3553639,
            Longitude = 8.9680407,
            Message = "Message",
            Image = "Image",
        };

        // Act
        var actual = albstone.FormatCoordinate();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AlbstoneFormatCoordinateDecimal()
    {
        // Arrange
        var expected = "48.355 8.968";
        var albstone = new Albstone()
        {
            Address = "Address",
            Name = "Name",
            Date = new DateTime(1972, 01, 09, 08, 00, 00),
            Latitude = 48.3553639,
            Longitude = 8.9680407,
            Message = "Message",
            Image = "Image",
        };

        // Act
        var actual = albstone.FormatCoordinate(true);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AlbstoneParse()
    {
        // Arrange
        var json = File.ReadAllText("Testdata/albstone.json");
        var expected = new Albstone()
        {
            Address = "Address",
            Name = "Name",
            Date = new DateTime(1972, 01, 09, 08, 00, 00),
            Latitude = 48.3553639,
            Longitude = 8.9680407,
            Message = "Message",
            Image = "Image",
        };

        // Act
        var actual = Albstone.Parse(json);

        // Assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void AlbstoneParseWithImage()
    {
        // Arrange
        var json = File.ReadAllText("Testdata/albstone128x128.json");
        var image = "Testdata/albstone128x128.png";
        var expected = new Albstone()
        {
            Address = "Address",
            Name = "Name",
            Date = new DateTime(1972, 01, 09, 08, 00, 00),
            Latitude = 48.3553639,
            Longitude = 8.9680407,
            Message = "Message",
            Image = "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",
        };

        // Act
        var actual = Albstone.Parse(json, image);

        // Assert
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void AlbstoneFaker()
    {
        // Arrange
        var albstone0 = File.ReadAllText("Testdata/albstone0.json");
        var albstone1 = File.ReadAllText("Testdata/albstone1.json");
        var albstone2 = File.ReadAllText("Testdata/albstone2.json");

        Randomizer.Seed = new Random(420);
        var millenium = new DateTime(2000, 1, 1, 0, 0, 0);

        // Act
        var albstoneFaker = new Faker<Albstone>("de")
            .RuleFor(a => a.Name, f => f.Name.FirstName(f.Person.Gender))
            .RuleFor(a => a.Date, f => f.Date.Future(20, millenium))
            .RuleFor(a => a.Latitude, f => f.Address.Latitude())
            .RuleFor(a => a.Longitude, f => f.Address.Longitude())
            .RuleFor(a => a.Message, f => f.Hacker.Phrase())
            .RuleFor(a => a.Image, AlbstoneRepository.DefaultImage);

        var albstones = albstoneFaker.Generate(3);

        foreach (var albstone in albstones)
        {
            var coordinate = new Coordinate(albstone.Latitude, albstone.Longitude, albstone.Date);
            var mnemonic = Magic.Mnemonic(albstone.Name, coordinate);
            var seed = Magic.SeedHex(mnemonic);
            albstone.Address = Magic.Address(seed, 0);
        }

        foreach (var albstone in albstones)
        {
            Log.Information("{albstone}", albstone.ToJson());
        }

        // Assert
        Assert.Equal(albstone0, albstones[0].ToJson());
        Assert.Equal(albstone1, albstones[1].ToJson());
        Assert.Equal(albstone2, albstones[2].ToJson());
    }
}
