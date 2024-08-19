using Albstones.WebApp.Helpers;
using CoordinateSharp;

namespace WebApp.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestHansi9172()
        {
            // Arrange
            string name = "Hansi";
            DateTime dob = new DateTime(1972, 01, 09);
            Coordinate coordinate = new Coordinate(48.3553639, 8.9680407, dob);

            // Act
            string[] mnemonic = Magic.Mnemonic(name, coordinate);
            string entropy = Magic.Entropy(mnemonic);
            string zodiac = coordinate.CelestialInfo.AstrologicalSigns.ZodiacSign.ToString();

            // Assert
            Assert.Equal("Capricorn", zodiac);
            Assert.Equal(@"N 48º 21' 19,31"" E 8º 58' 4,947""", coordinate.ToString());
            Assert.Equal("6820024aded7023f4004f5b02e047ecc", entropy);
        }

        [Fact]
        public void TestLuca854()
        {
            // Arrange
            string name = "Luca";
            DateTime dob = new DateTime(2008, 05, 08);
            Coordinate coordinate = new Coordinate(48.214091, 9.0190494, dob);

            // Act
            string[] mnemonic = Magic.Mnemonic(name, coordinate);
            string entropy = Magic.Entropy(mnemonic);
            string zodiac = coordinate.CelestialInfo.AstrologicalSigns.ZodiacSign.ToString();

            // Assert
            Assert.Equal("Taurus", zodiac);
            Assert.Equal(@"N 48º 12' 50,728"" E 9º 1' 8,578""", coordinate.ToString());
            Assert.Equal("7c1d807e800dce003b0581ec17b4003e", entropy);
        }

        [Fact]
        public void TestMnemonic()
        {
            // Arrange
            string name = "Mnemonic";
            DateTime dob = new DateTime(2024, 08, 19);
            Coordinate coord = new Coordinate(0.0, 0.0, dob);

            // Act
            string[] wordlist = Magic.Mnemonic(name, coord);
            string mnemonic = string.Join(' ', wordlist);

            // Assert
            Assert.Equal("machine naive eager machine oak naive ice cabbage lab eager oak abuse", mnemonic);
        }
    }
}
