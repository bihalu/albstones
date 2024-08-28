using CoordinateSharp;
using dotnetstandard_bip39;
using System.Reflection;

namespace Albstones.WebApp.Helpers
{
    public static class Magic
    {
        public static string Entropy(string[] word)
        {
            if (null == word) throw new ArgumentException("empty word list, can't get entropy");

            if (word.Count() != 12) throw new ArgumentException("need 12 words, can't get entropy");

            var bip39 = new BIP39();
            string mnemonic = string.Join(" ", word);
            return bip39.MnemonicToEntropy(mnemonic, BIP39Wordlist.English);
        }

        public static string[] Mnemonic(string name, Coordinate coordinate)
        {
            string[] word = new string[12];
            string magic = name + coordinate.CelestialInfo.AstrologicalSigns.ZodiacSign + "Albstones";
            magic = magic.ToLower();

            // get wordlist via private method
            Type type = typeof(BIP39);
            var getWordlist = Activator.CreateInstance(type);
            var method = type.GetMethod("GetWordlist", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = [BIP39Wordlist.English];
            string[] wordlist = (string[])method.Invoke(getWordlist, parameters);

            int counter = 0;
            foreach (char c in magic.Substring(0, 12))
            {
                word[counter++] = Array.Find(wordlist, w => w[0] == c);
            }

            // expensive exception and ugly goto ;-)
            repeat:
            try
            {
                BIP39 bip39 = new BIP39();
                string mnemonic = string.Join(" ", word);
                string entropy = bip39.MnemonicToEntropy(mnemonic, BIP39Wordlist.English);
            }
            catch (Exception)
            {
                int index = Array.IndexOf(wordlist, word[11]) + 1;
                if (index >= wordlist.Length) index = 0;
                word[11] = wordlist[index];
                goto repeat;
            }

            return word;
        }

        public static string[] Mnemonic(string entropy)
        {
            BIP39 bip39 = new BIP39();
            string[] words = bip39.EntropyToMnemonic(entropy, BIP39Wordlist.English).Split(' ');
        
            return words;
        }
    }
}
