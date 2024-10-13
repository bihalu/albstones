using System.Security.Cryptography;

namespace Albstones.Helper
{
    public class AesEncryption
    {

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] initializationVector)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            var encryptedData = aes.EncryptCbc(data, initializationVector);

            return encryptedData;
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] initializationVector)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            var decryptedData = aes.DecryptCbc(data, initializationVector);

            return decryptedData;
        }
    }
}
