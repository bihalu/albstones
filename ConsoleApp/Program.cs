using Albstones.Helper;
using Albstones.Models;
using CoordinateSharp;
using ExcelDataReader;
using Newtonsoft.Json;
using QRCoder;

namespace Albstones.ConsoleApp;

internal class Program
{
    static int Main(string[] args)
    {
        // Excel file name
        string filePath = "albstones.xlsx";

        List<Albstone> albstones = [];

        if (args.Length == 1)
        {
            filePath = args[0];
        }

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Can't open file " + filePath);
            Console.WriteLine("Create Fake Albstones");
            albstones = AlbstonesFromFakeData();
        }
        else
        {
            albstones = AlbstonesFromExcel(filePath);
        }

        // Write albstones.json
        File.WriteAllText("albstones.json", JsonConvert.SerializeObject(albstones, Formatting.Indented));

        return 0;
    }

    private static List<Albstone> AlbstonesFromFakeData()
    {
        List<Albstone> albstones = [];

        var fakeAlbstones = Albstone.FakeData(99);

        foreach (var item in fakeAlbstones)
        {
            Coordinate coordinate = new Coordinate(item.Latitude, item.Longitude, item.Date);
            string[] mnemonic = Magic.Mnemonic(item.Name!, coordinate);

            var albstone = CreateAlbstone(mnemonic, item.Address!, item.Name!, item.Message!, item.Image!, item.Date, item.Latitude, item.Longitude, out bool isValid);
            if (isValid)
            {
                albstones.Add(albstone);
            }
        }

        return albstones;
    }

    private static List<Albstone> AlbstonesFromExcel(string filePath)
    {
        List<Albstone> albstones = [];

        // Fix No data is available for encoding 1252
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            var result = reader.AsDataSet();

            var sheet = result.Tables[0].DataSet!.Tables[0];

            for (int i = 1; i < sheet.Rows.Count; i++)
            {
                string[] word = new string[12];
                for (int j = 0; j < word.Length; j++)
                {
                    word[j] = (string)sheet.Rows[i][j];
                }

                string address, name, message, imagePath;
                DateTime date;
                double latitude, longitude;

                // Read Excel data
                try
                {
                    address = (string)sheet.Rows[i][12];
                    date = (DateTime)sheet.Rows[i][13];
                    name = (string)sheet.Rows[i][14];
                    latitude = (Double)sheet.Rows[i][15];
                    longitude = (Double)sheet.Rows[i][16];
                    message = (string)sheet.Rows[i][17];
                    imagePath = (string)sheet.Rows[i][18];
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error reading Excel row " + i + ": " + exception.Message);
                    continue;
                }

                var albstone = CreateAlbstone(word, address, name, message, imagePath, date, latitude, longitude, out bool isValid);

                if (isValid)
                {
                    albstones.Add(albstone);
                }
            }

        }

        return albstones;
    }

    private static Albstone CreateAlbstone(string[] word, string address, string name, string message, string imagePath, DateTime date, double latitude, double longitude, out bool isValid)
    {
        var albstone = new Albstone();
        isValid = false;

        // Derive address0 from words
        string address0;
        try
        {
            string seedHex = Magic.SeedHex(word);
            address0 = Magic.Address(seedHex, 0);
        }
        catch (Exception exception)
        {
            Console.WriteLine("Can't derive address from words: " + exception.Message);
            return albstone;
        }

        // Check address
        if (address != address0)
        {
            Console.WriteLine("Calculated address is different: " + address0 + " - " + address);
            return albstone;
        }

        string image;

        // Image already in base64
        if (IsBase64String(imagePath))
        {
            image = imagePath;
        }
        else
        {
            // Convert image to base64
            if (!File.Exists(imagePath))
            {
                Console.WriteLine("Can't open image " + imagePath);
                return albstone;
            }

            byte[] byteArray = File.ReadAllBytes(imagePath);
            image = Convert.ToBase64String(byteArray);
        }

        albstone.Address = address;
        albstone.Name = name;
        albstone.Date = date;
        albstone.Latitude = latitude;
        albstone.Longitude = longitude;
        albstone.Message = message;
        albstone.Image = image;
        isValid = true;

        Console.WriteLine("QRCode for albstone " + name);

        var scanBytes = System.Text.Encoding.UTF8.GetBytes(string.Join(' ', word));
        var scan = Convert.ToBase64String(scanBytes);
        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode(scan, QRCodeGenerator.ECCLevel.H);
        var asciiCode = new AsciiQRCode(data).GetGraphicSmall(invert: false);
        Console.WriteLine(asciiCode);

        var data2 = gen.CreateQrCode(scan, QRCodeGenerator.ECCLevel.H);
        var pngCode = new PngByteQRCode(data2).GetGraphic(5);

        File.WriteAllBytes(address + ".png", pngCode);

        return albstone;
    }

    private static bool IsBase64String(string base64)
    {
        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
    }
}
