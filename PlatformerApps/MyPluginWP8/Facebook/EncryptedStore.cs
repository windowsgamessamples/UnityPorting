using System.IO.IsolatedStorage;
using Legacy.Encryption;
using LegacySystem.IO;
using Windows.ApplicationModel.Store;
using Windows.Storage;

namespace MyPlugin.Facebook
{
    internal static class EncryptedStore
    {
        public static void SaveSetting(string key, string setting)
        {
            var encrypted = EncryptionProvider.EncryptString(setting, CurrentApp.AppId.ToString());
            IsolatedStorageSettings.ApplicationSettings.Add(key, encrypted);
        }

        public static string LoadSetting(string key)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(key))
                return null;
            
            return EncryptionProvider.DecryptString(IsolatedStorageSettings.ApplicationSettings[key].ToString(), CurrentApp.AppId.ToString());
        }

        public static void Remove(string key)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = null;
            IsolatedStorageSettings.ApplicationSettings.Remove(key);
        }
    }
}
