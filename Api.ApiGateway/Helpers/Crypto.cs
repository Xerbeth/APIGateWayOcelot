using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Api.ApiGateway.Helpers
{
    public static class Crypto
    {
        static string SecretKey = Startup.StaticConfig.GetSection("AESOptions").GetSection("SecretKey").Value;
        static Crypto(){}

        public static string EncryptString(string plainText)
        {
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var iv = Encoding.UTF8.GetBytes(SecretKey);
            byte[] encrypted;

            // Check arguments.
            if (String.IsNullOrEmpty(plainText))
                return string.Empty;
            // Create a RijndaelManaged object with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptString(string textEncrypted)
        {
            
            var encrypted = Convert.FromBase64String(textEncrypted);
            // Declare the string used to hold the decrypted text.
            string plainText = string.Empty;
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var iv = Encoding.UTF8.GetBytes(SecretKey);
            // Check arguments.
            if (encrypted == null || encrypted.Length <= 0)
                return plainText;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(encrypted))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream and place them in a string.
                                plainText = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new ArgumentException("Ocurrió un error descrifrando la información");
                }
            }
            return plainText;
        }        
    }
}
