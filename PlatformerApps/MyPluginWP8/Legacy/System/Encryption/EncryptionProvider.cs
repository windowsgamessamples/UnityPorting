using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;

namespace Legacy.Encryption
{
    /// <summary>
    /// Use the Data Protection API (DPAPI) to encrypt/decrypt data using the built-in user and device credentials 
    /// This is more secure than attempting to store the encryption key on the phone manually.
    /// </summary>
    public class EncryptionProvider
    {
        public static void EncryptString(string key, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] protectedBytes = ProtectedData.Protect(bytes, null);

            var file = IsolatedStorageFile.GetUserStoreForApplication();
            var writeStream = new IsolatedStorageFileStream(key, FileMode.Create, FileAccess.Write, file);

            Stream writer = new StreamWriter(writeStream).BaseStream;
            writer.Write(protectedBytes, 0, protectedBytes.Length);
            writer.Close();
            writeStream.Close();
        }

        public static string DecryptString(string key)
        {
            var file = IsolatedStorageFile.GetUserStoreForApplication();
            if (!file.FileExists(key))
                return null;

            var readStream = new IsolatedStorageFileStream(key, FileMode.Open, FileAccess.Read, file);

            var reader = new StreamReader(readStream).BaseStream;
            byte[] protectedBytes = new byte[reader.Length];

            reader.Read(protectedBytes, 0, protectedBytes.Length);
            reader.Close();
            readStream.Close();

            byte[] unprotectedBytes = ProtectedData.Unprotect(protectedBytes, null);
            return Encoding.UTF8.GetString(unprotectedBytes, 0, unprotectedBytes.Length);

        }
    }
}
