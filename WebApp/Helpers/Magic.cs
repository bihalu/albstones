using CoordinateSharp;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace Albstones.WebApp.Helpers
{
    public static class Magic
    {
        public static string SeedHex(string[] word, string password = "")
        {
            if (null == word) throw new ArgumentException("empty word list, can't get entropy");

            if (word.Count() != 12) throw new ArgumentException("need 12 words, can't get entropy");

            var mnemonic = new Mnemonic(string.Join(' ', word), Wordlist.English);
            return Encoders.Hex.EncodeData(mnemonic.DeriveSeed(password));
        }


        public static string[] Mnemonic(string name, Coordinate coordinate)
        {
            string[] word = new string[12];
            string magic = name + coordinate.CelestialInfo.AstrologicalSigns.ZodiacSign + "Albstones";
            magic = magic.ToLower();

            var wordlist = Wordlist.English.GetWords().ToArray();

            int counter = 0;
            foreach (char c in magic.Substring(0, 12))
            {
                word[counter++] = wordlist.First(w => w[0] == c);
            }

            var mnemonic = new Mnemonic(string.Join(' ', word), Wordlist.English);

            while(!mnemonic.IsValidChecksum)
            {
                int index = Array.IndexOf(wordlist, word[11]) + 1;
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
}
