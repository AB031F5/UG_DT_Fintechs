using System.Security.Cryptography;
using System.Text;

namespace UG_DT_InternalAmolCall.Logic
{
    public class EncryptionManager
    {
        private const string EncryptionKey = "YBmHi0ZwIg6Bl3CL";
        public static string DecryptString(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            string decryptedText;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aesAlg.IV = new byte[16]; // Initialization Vector (IV) should be same as used for encryption
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            decryptedText = reader.ReadToEnd();
                        }
                    }
                }
            }

            return decryptedText;
        }
        public static string EncryptString(string request)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(request);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
    }
}
