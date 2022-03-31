using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Services
{
    public class EncryptionService
    {
        private const string _secretKey = "$P3tSh0p_Encryp1t4t10n**";

        public EncryptionService(IServiceProvider serviceProvider)
        { }

        public static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Encripta un dato
        /// </summary>
        public static string Encrypt(object data)
        {
            string plainText = data?.ToString();

            if (string.IsNullOrEmpty(plainText)) return plainText;

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_secretKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Desencripta un dato
        /// </summary>
        public static TResult Decrypt<TResult>(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText) || cipherText.Equals("undefined"))
                return default(TResult);

            string result = null;
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_secretKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            if ((result == null || result == "") && typeof(TResult).Name.ToLower().Contains("nullable"))
                return default(TResult);

            Type notNullableTResult = Nullable.GetUnderlyingType(typeof(TResult)) ?? typeof(TResult);
            return (TResult)Convert.ChangeType(result, notNullableTResult);
        }
    }
}
