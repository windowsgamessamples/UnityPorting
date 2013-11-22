using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.Encryption
{
    public class EncryptionProvider
    {

        public static byte[] GetMD5Hash(string input)
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            MD5Managed md5 = new MD5Managed();
            return md5.ComputeHash(bs);

            //StringBuilder sb = new StringBuilder();
            //foreach (byte b in hash)
            //{
            //    sb.Append(b.ToString("x2").ToLower());
            //}

            //return sb.ToString();
        }

        private static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        public static string EncryptString(string Value, string KeyString)
        {
            byte[] Key = DurianBerryProducts.DESCrytography.DoPadWithString(encoding.GetBytes(KeyString), 24, (byte)0);
            byte[] plainText = new byte[1024];
            plainText = DurianBerryProducts.DESCrytography.DoPadWithString(encoding.GetBytes(Value), 8, (byte)0);

            byte[] cipherText = null;
            DurianBerryProducts.DESCrytography.TripleDES(plainText, ref cipherText, Key, true);

            string result = Convert.ToBase64String(cipherText);
            result = result.Replace("+", "-").Replace("/", "_");

            return result;
        }

        public static string DecryptString(string Value, string KeyString)
        {
            byte[] Key = DurianBerryProducts.DESCrytography.DoPadWithString(encoding.GetBytes(KeyString), 24, (byte)0);
            byte[] plainText = Convert.FromBase64String(Value.Replace("-", "+").Replace("_", "/"));

            byte[] cipherText = null;
            DurianBerryProducts.DESCrytography.TripleDES(plainText, ref cipherText, Key, false);

            string result = encoding.GetString(cipherText, 0, cipherText.Length).Replace(Convert.ToChar(0x0).ToString(), "");

            return result;
        }
    }
}
