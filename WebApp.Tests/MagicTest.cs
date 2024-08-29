using Albstones.WebApp.Helpers;
using CoordinateSharp;

namespace WebApp.Tests
{
    public class MagicTest
    {
        [Fact]
        public void MnemonicHansi9172()
        {
            // Arrange
            string name = "Hansi";
            DateTime dob = new DateTime(1972, 01, 09);
            Coordinate coordinate = new Coordinate(48.3553639, 8.9680407, dob);
            CoordinateFormatOptions decimal7 = new CoordinateFormatOptions()
            {
                Format = CoordinateFormatType.Decimal,
                Display_Leading_Zeros = false,
                Round = 7,
            };

            // Act
            string[] mnemonic = Magic.Mnemonic(name, coordinate);
            string words = string.Join(' ', mnemonic);
            string zodiac = coordinate.CelestialInfo.AstrologicalSigns.ZodiacSign.ToString();

            // Assert
            Assert.Equal("Capricorn", zodiac);
            Assert.Equal(@"N 48º 21' 19,31"" E 8º 58' 4,947""", coordinate.ToString());
            Assert.Equal("48,3553639 8,9680407", coordinate.ToString(decimal7));
            Assert.Equal("habit abandon naive sad ice cabbage abandon pact rabbit ice cabbage observe", words);
        }

        [Fact]
        public void MnemonicLuca854()
        {
            // Arrange
            string name = "Luca";
            DateTime dob = new DateTime(2008, 05, 08);
            Coordinate coordinate = new Coordinate(48.214091, 9.0190494, dob);
            CoordinateFormatOptions decimal7 = new CoordinateFormatOptions()
            {
                Format = CoordinateFormatType.Decimal,
                Display_Leading_Zeros = false,
                Round = 7,
            };

            // Act
            string[] mnemonic = Magic.Mnemonic(name, coordinate);
            string words = string.Join(' ', mnemonic);
            string zodiac = coordinate.CelestialInfo.AstrologicalSigns.ZodiacSign.ToString();

            // Assert
            Assert.Equal("Taurus", zodiac);
            Assert.Equal(@"N 48º 12' 50,728"" E 9º 1' 8,578""", coordinate.ToString());
            Assert.Equal("48,214091 9,0190494", coordinate.ToString(decimal7));
            Assert.Equal("lab ugly cabbage abandon table abandon ugly rabbit ugly sad abandon lab", words);
        }

        [Fact]
        public void MnemonicFromNameCoord()
        {
            // Arrange
            string name = "Mnemonic";
            DateTime dob = new DateTime(2024, 08, 19);
            Coordinate coordinate = new Coordinate(0.0, 0.0, dob);

            // Act
            string[] words = Magic.Mnemonic(name, coordinate);
            string mnemonic = string.Join(' ', words);

            // Assert
            Assert.Equal("machine naive eager machine oak naive ice cabbage lab eager oak abuse", mnemonic);
        }

        [Fact]
        public void SeedHexFromMnemonic()
        {
            // Arrange
            string words = "machine naive eager machine oak naive ice cabbage lab eager oak abuse";

            // Act
            string seedHex = Magic.SeedHex(words.Split(' '));

            // Assert
            Assert.Equal("9ab8459549177e9458fbe3a70446f76dd3857379b1e289653c58ed8065d6d68b20c4061722def81bcc4a4dcd33974cc318a2dac0198faf89874c9544de3cd313", seedHex);
        }

        [Fact]
        public void MnemonicFromEntropy()
        {
            // Arrange
            string entropy = "85925513c2c97d255c08fd7c089e5f00";

            // Act
            string[] words = Magic.Mnemonic(entropy);
            string mnemonic = string.Join(' ', words);

            // Assert
            Assert.Equal("machine naive eager machine oak naive ice cabbage lab eager oak abuse", mnemonic);
        }
    }
}
