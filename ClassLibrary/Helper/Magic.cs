using CoordinateSharp;
using NBitcoin;
using NBitcoin.DataEncoders;
using System.Text.RegularExpressions;

namespace Albstones.Helper;

// Bitcoin magic see -> https://iancoleman.io/bip39/
public static class Magic
{
    public static string SeedHex(string[] word, string password = "")
    {
        var mnemonic = new Mnemonic(string.Join(' ', word), Wordlist.English);

        return Encoders.Hex.EncodeData(mnemonic.DeriveSeed(password));
    }

    public static string RootKey(string seedHex)
    {
        var seed = Encoders.Hex.DecodeData(seedHex);
        var key = ExtKey.CreateFromSeed(seed);

        return key.GetWif(Network.Main).ToString();
    }

    public static string Address(string seedHex, int index = 0, ScriptPubKeyType type = ScriptPubKeyType.Segwit)
    {
        var seed = Encoders.Hex.DecodeData(seedHex);
        var rootKey = ExtKey.CreateFromSeed(seed);

        var keyPath = KeyPath.Parse("44'/0'/0'/" + index);

        var extPrivKey = rootKey.Derive(keyPath).GetWif(Network.Main);
        var address = rootKey.Derive(keyPath).GetPublicKey().GetAddress(type, Network.Main);

        return address.ToString();
    }

    public static string RootKey(string[] word, string password = "")
    {
        var mnemonic = new Mnemonic(string.Join(' ', word), Wordlist.English);

        return mnemonic.DeriveExtKey(password).ToString(Network.Main);
    }

    public static string[] Mnemonic(string name, Coordinate coordinate)
    {
        var word = new string[12];
        var moonName = coordinate.CelestialInfo.AlmanacMoonName.Name;
        var magic = name + moonName + "Albstones";
        magic = magic.ToLower();
        magic = magic.Replace("x", ""); // English word list contains no word with x
        magic = Regex.Replace(magic, @"[^\u0000-\u007F]+", ""); // Remove none ascii characters

        var wordlist = Wordlist.English.GetWords().ToArray();

        var counter = 0;
        foreach (var c in magic.Substring(0, 12))
        {
            word[counter++] = wordlist.First(w => w[0] == c);
        }

        var mnemonic = new Mnemonic(string.Join(' ', word), Wordlist.English);

        while (!mnemonic.IsValidChecksum)
        {
            var index = Array.IndexOf(wordlist, word[11]) + 1;
            if (index >= wordlist.Length) index = 0;
            word[11] = wordlist[index];
            mnemonic = new Mnemonic(string.Join(' ', word), Wordlist.English);
        }

        return word;
    }

    public static string[] Mnemonic(string entropy)
    {
        var entropyBytes = Encoders.Hex.DecodeData(entropy);
        var mnemonic = new Mnemonic(Wordlist.English, entropyBytes);

        return mnemonic.Words;
    }
}
