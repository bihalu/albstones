using Albstones.Helpers;
using Albstones.Models;
using ExcelDataReader;
using Newtonsoft.Json;
using QRCoder;

namespace Albstones.ConsoleApp;

internal class Program
{
    static int Main(string[] args)
    {
        string filePath = "albstones.xlsx";

        if (args.Length == 1)
        {
            filePath = args[0];
        }

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Can't open file " + filePath);
            return 1;
        }

        // Fix No data is available for encoding 1252
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            List<Albstone> albstones = new();

            var result = reader.AsDataSet();

            var sheet = result.Tables[0].DataSet!.Tables[0];

            for (int i = 1; i < sheet.Rows.Count; i++)
            {
                string[] word = new string[12];
                for (int j = 0; j < word.Length; j++)
                {
                    word[j] = (string)sheet.Rows[i][j];
                }

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
                    continue;
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

                // Check address
                if (address != address0)
                {
                    Console.WriteLine("Calculated address is different: " + address0 + " - " + address);
                    continue;
                }

                // Convert image to base64
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Can't open image " + imagePath);
                    continue;
                }

                byte[] byteArray = File.ReadAllBytes(imagePath);
                string image = Convert.ToBase64String(byteArray);

                var albstone = new Albstone()
                {
                    Address = address,
                    Name = name,
                    Date = date,
                    Latitude = latitude,
                    Longitude = longitude,
                    Message = message,
                    Image = image,
                };

                Console.WriteLine("Add Albstone " + name);
                albstones.Add(albstone);

                var gen = new QRCodeGenerator();
                var data = gen.CreateQrCode(string.Join(' ', word), QRCodeGenerator.ECCLevel.Q);
                var asciiCode = new AsciiQRCode(data).GetGraphicSmall(invert: false);
                Console.WriteLine(asciiCode);

                var data2 = gen.CreateQrCode(string.Join(' ', word), QRCodeGenerator.ECCLevel.L);
                var pngCode = new PngByteQRCode(data2).GetGraphic(5);

                File.WriteAllBytes(address + ".png", pngCode);
            }

            // Write albstones.json
            File.WriteAllText("albstones.json", JsonConvert.SerializeObject(albstones, Formatting.Indented));
        }

        return 0;
    }
}
