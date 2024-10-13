using System.Security.Cryptography;
using Albstones.Helper;
using CoordinateSharp;

namespace XunitTests;

public class MagicTest
{
    [Fact]
    public void MnemonicFromNameCoord()
    {
        // Arrange
        var name = "Mnemonic";
        var dob = new DateTime(2024, 08, 19);
        var coordinate = new Coordinate(0.0, 0.0, dob);

        // Act
        var words = Magic.Mnemonic(name, coordinate);
        var mnemonic = string.Join(' ', words);

        // Assert
        Assert.Equal("machine naive eager machine oak naive ice cabbage sad table ugly race", mnemonic);
    }

    [Fact]
    public void SeedHexFromMnemonic()
    {
        // Arrange
        var words = "machine naive eager machine oak naive ice cabbage lab eager oak abuse";

        // Act
        var seedHex = Magic.SeedHex(words.Split(' '));

        // Assert
        Assert.Equal("9ab8459549177e9458fbe3a70446f76dd3857379b1e289653c58ed8065d6d68b20c4061722def81bcc4a4dcd33974cc318a2dac0198faf89874c9544de3cd313", seedHex);
    }

    [Fact]
    public void RootKeyFromSeed()
    {
        // Arrange
        var seed = "9ab8459549177e9458fbe3a70446f76dd3857379b1e289653c58ed8065d6d68b20c4061722def81bcc4a4dcd33974cc318a2dac0198faf89874c9544de3cd313";

        // Act
        var rootKey = Magic.RootKey(seed);

        // Assert
        Assert.Equal("xprv9s21ZrQH143K41pGUKurFpCEZJbA26HjZ2DEfTsVJReMDMbeP4bcTiFeL8WWYkHY1zmkcy4hMjXVQtcAseGGKHdn52YstaYu8teP2jSyAvy", rootKey);
    }

    [Fact]
    public void AddressFromSeed()
    {
        // Arrange
        var seed = "9ab8459549177e9458fbe3a70446f76dd3857379b1e289653c58ed8065d6d68b20c4061722def81bcc4a4dcd33974cc318a2dac0198faf89874c9544de3cd313";

        // Act
        var segwitAddress0 = Magic.Address(seed, 0);
        var segwitAddress1 = Magic.Address(seed, 1);
        var segwitAddress2 = Magic.Address(seed, 2);
        var segwitAddress9 = Magic.Address(seed, 9);

        var legacyAddress0 = Magic.Address(seed, 0, NBitcoin.ScriptPubKeyType.Legacy);
        var legacyAddress1 = Magic.Address(seed, 1, NBitcoin.ScriptPubKeyType.Legacy);
        var legacyAddress2 = Magic.Address(seed, 2, NBitcoin.ScriptPubKeyType.Legacy);
        var legacyAddress9 = Magic.Address(seed, 9, NBitcoin.ScriptPubKeyType.Legacy);

        // Assert
        Assert.Equal("bc1q08xg063864v5xw6tuctkmleap9elf7hua6q6km", segwitAddress0);
        Assert.Equal("bc1qjg3ljnn9jalhldc29wnykwj39u4kxlcuwnjff4", segwitAddress1);
        Assert.Equal("bc1qxd74qn44d0gqv8xztny3ryy4st2zdrmw2xzd4t", segwitAddress2);
        Assert.Equal("bc1qgj9u5gsa38ys2vfcznjt2gk4ejdscln6333vew", segwitAddress9);

        Assert.Equal("1C71nGawDn8gsav7CczSDunj9CydsY1Qn9", legacyAddress0);
        Assert.Equal("1EKikWqDGugweU9PVdBfApyeFR3j6ZyBcQ", legacyAddress1);
        Assert.Equal("15hFajTFFjNHynN8Qr7BhXFK1Lb37ELgMf", legacyAddress2);
        Assert.Equal("17FSQo7JFcTZ8aUiHnhniM1eVhfNfU8wbB", legacyAddress9);
    }

    [Fact]
    public void RootKeyFromMnemonic()
    {
        // Arrange
        var words = "machine naive eager machine oak naive ice cabbage lab eager oak abuse";

        // Act
        var rootKey = Magic.RootKey(words.Split(' '));

        // Assert
        Assert.Equal("xprv9s21ZrQH143K41pGUKurFpCEZJbA26HjZ2DEfTsVJReMDMbeP4bcTiFeL8WWYkHY1zmkcy4hMjXVQtcAseGGKHdn52YstaYu8teP2jSyAvy", rootKey);
    }

    [Fact]
    public void MnemonicFromEntropy()
    {
        // Arrange
        var entropy = "85925513c2c97d255c08fd7c089e5f00";

        // Act
        var words = Magic.Mnemonic(entropy);
        var mnemonic = string.Join(' ', words);

        // Assert
        Assert.Equal("machine naive eager machine oak naive ice cabbage lab eager oak abuse", mnemonic);
    }

    [Fact]
    public void AesEncryptionDecryption()
    {
        // Arrange
        var message = "Hello world!";
        var initializationVector = RandomNumberGenerator.GetBytes(16);
        var key = RandomNumberGenerator.GetBytes(32);

        // Act
        var encryptedData = AesEncryption.Encrypt(message.ToBytes(), key, initializationVector);
        var decryptedData = AesEncryption.Decrypt(encryptedData, key, initializationVector);

        // Assert
        Assert.Equal(message, decryptedData.BytesToString());
    }

    [Theory]
    [InlineData("Hello world!", 0, "Hello world!")]
    [InlineData("Hello world!", 4, "Hell")]
    [InlineData("Hello world!", 8, "Hello wo")]
    [InlineData("Hello world!", 16, "Hello world!    ")]
    [InlineData("Hello world!", 32, "Hello world!                    ")]
    public void StringToBytes(string message, int count, string expected)
    {
        // Arrange
        int expectedCount = count > 0 ? count : message.Length;

        // Act
        var bytes = message.ToBytes(count);
        var actual = bytes.BytesToString();

        // Assert
        Assert.Equal(expectedCount, actual.Length);
        Assert.Equal(expected, actual);
    }
}
