using System.IO.IsolatedStorage;
using Legacy.Encryption;

namespace MyPlugin.Facebook
{
    internal static class EncryptedStore
    {
        public static void SaveSetting(string key, string value)
        {
            EncryptionProvider.EncryptString(key, value);
        }

        public static string LoadSetting(string key)
        {
            return EncryptionProvider.DecryptString(key);
        }

        public static void Remove(string key)
        {
            EncryptionProvider.RemoveKey(key);
        }
    }
}
