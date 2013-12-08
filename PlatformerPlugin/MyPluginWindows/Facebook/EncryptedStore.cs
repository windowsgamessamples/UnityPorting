using LegacySystem.IO;
using Windows.ApplicationModel.Store;
using Windows.Storage;

namespace MyPlugin.Facebook
{
    internal static class EncryptedStore
    {
        public static void SaveSetting(string key, string setting)
        {
            var encrypted = EncryptionProvider.Encrypt(setting, CurrentApp.AppId.ToString());
            ApplicationData.Current.LocalSettings.Values[key] = encrypted;
        }

        public static string LoadSetting(string key)
        {
            var encrypted = ApplicationData.Current.LocalSettings.Values[key] as string;
            if (encrypted == null) return null;
            return EncryptionProvider.Decrypt(encrypted, CurrentApp.AppId.ToString());
        }

        public static void Remove(string key)
        {
            ApplicationData.Current.LocalSettings.Values[key] = null;
            ApplicationData.Current.LocalSettings.Values.Remove(key);
        }
    }
}
