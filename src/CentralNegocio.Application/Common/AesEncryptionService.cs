using System.Security.Cryptography;
using System.Text;

namespace CentralNegocio.Application.Common
{
    public class AesEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        private readonly int _ivSize;

        public AesEncryptionService(string key)
        {
            _key = Encoding.UTF8.GetBytes(key); // Debe ser de 32 bytes para AES-256
        }

        public string Encrypt(string plainText)
        {
            string strCifrado = plainText;
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.GenerateIV(); // Genera un IV aleatorio
                byte[] iv = aes.IV;

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length); // Escribir el IV al principio del stream
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    strCifrado = Convert.ToBase64String(ms.ToArray());
                }
            }
            return strCifrado;
        }

        public string DecryptDataUrl(string encodedCipherText)
        {
            try
            {
                // Decodificar la cadena URL encoded antes de descifrar
                string cipherText = encodedCipherText; //HttpUtility.UrlDecode(encodedCipherText);

                byte[] fullCipher = FromBase64Url(cipherText);
                byte[] iv = new byte[16]; // Tamaño del IV para AES es 128 bits (16 bytes)
                byte[] cipher = new byte[fullCipher.Length - iv.Length];
                string strData = string.Empty;

                // Extraer el IV
                Array.Copy(fullCipher, iv, iv.Length);
                // Extraer el texto cifrado
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC; // Establecer el modo a CBC
                    aes.Key = _key;
                    aes.IV = iv;

                    using (MemoryStream ms = new MemoryStream(cipher))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                strData = sr.ReadToEnd();
                            }
                        }
                    }
                }
                return strData;
            }
            catch
            {
                return encodedCipherText;
            }
        }
        // Convierte de Base64URL a byte array
        private byte[] FromBase64Url(string input)
        {
            string base64 = input
                .Replace('-', '+')
                .Replace('_', '/');

            // Rellenar con `=` para hacer que la longitud sea un múltiplo de 4
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }

        // Convierte a Base64URL
        string ToBase64Url(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", string.Empty); // Eliminar `=`
        }

        public string DecryptDataBody(string encodedCipherText)
        {
            try
            {
                string cipherText = encodedCipherText; //HttpUtility.UrlDecode(encodedCipherText);

                byte[] fullCipher = Convert.FromBase64String(cipherText);
                byte[] iv = new byte[16]; // Tamaño del IV para AES es 128 bits (16 bytes)
                byte[] cipher = new byte[fullCipher.Length - iv.Length];
                string strData = string.Empty;

                // Extraer el IV
                Array.Copy(fullCipher, iv, iv.Length);
                // Extraer el texto cifrado
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC; // Establecer el modo a CBC
                    aes.Key = _key;
                    aes.IV = iv;

                    using (MemoryStream ms = new MemoryStream(cipher))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                strData = sr.ReadToEnd();
                            }
                        }
                    }
                }
                return strData;
            }
            catch
            {
                return encodedCipherText;
            }
        }
    }
}
