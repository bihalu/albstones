using Albstones.Helpers;
using CoordinateSharp;

namespace WebApp.Tests;

// https://iancoleman.io/bip39/
public class MagicTest
{
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
        Assert.Equal("machine naive eager machine oak naive ice cabbage sad table ugly race", mnemonic);
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
    public void RootKeyFromSeed()
    {
        // Arrange
        string seed = "9ab8459549177e9458fbe3a70446f76dd3857379b1e289653c58ed8065d6d68b20c4061722def81bcc4a4dcd33974cc318a2dac0198faf89874c9544de3cd313";

        // Act
        string rootKey = Magic.RootKey(seed);

        // Assert
        Assert.Equal("xprv9s21ZrQH143K41pGUKurFpCEZJbA26HjZ2DEfTsVJReMDMbeP4bcTiFeL8WWYkHY1zmkcy4hMjXVQtcAseGGKHdn52YstaYu8teP2jSyAvy", rootKey);
    }

    [Fact]
    public void AddressFromSeed()
    {
        // Arrange
        string seed = "9ab8459549177e9458fbe3a70446f76dd3857379b1e289653c58ed8065d6d68b20c4061722def81bcc4a4dcd33974cc318a2dac0198faf89874c9544de3cd313";

        // Act
        string segwitAddress0 = Magic.Address(seed, 0);
        string segwitAddress1 = Magic.Address(seed, 1);
        string segwitAddress2 = Magic.Address(seed, 2);
        string segwitAddress9 = Magic.Address(seed, 9);

        string legacyAddress0 = Magic.Address(seed, 0, NBitcoin.ScriptPubKeyType.Legacy);
        string legacyAddress1 = Magic.Address(seed, 1, NBitcoin.ScriptPubKeyType.Legacy);
        string legacyAddress2 = Magic.Address(seed, 2, NBitcoin.ScriptPubKeyType.Legacy);
        string legacyAddress9 = Magic.Address(seed, 9, NBitcoin.ScriptPubKeyType.Legacy);

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
        string words = "machine naive eager machine oak naive ice cabbage lab eager oak abuse";

        // Act
        string rootKey = Magic.RootKey(words.Split(' '));

        // Assert
        Assert.Equal("xprv9s21ZrQH143K41pGUKurFpCEZJbA26HjZ2DEfTsVJReMDMbeP4bcTiFeL8WWYkHY1zmkcy4hMjXVQtcAseGGKHdn52YstaYu8teP2jSyAvy", rootKey);
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
