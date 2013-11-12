using System;
using System.IO;
#if NETFX_CORE
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
#endif

namespace MyPlugin.WACK.System.IO
{
    public static class DirectoryNative
    {

        public static string GetAppDirectory()
        {
#if NETFX_CORE
            return Package.Current.InstalledLocation.Path;
#else
            throw new NotImplementedException();
#endif
        }

        public static string[] GetFiles(string path)
        {
#if NETFX_CORE
            var t = GetFilesAsync(path.Replace('/', '\\'));
            t.Wait();
            return t.Result;
#else
            throw new NotImplementedException();
#endif
        }

        public static bool CreateDirectory(string folderName)
        {
#if NETFX_CORE
            var t = CreateDirectoryAsync(folderName);
            t.Wait();
            if (t.IsCompleted)
                return t.Result;
            else
                return false;
#else
            throw new NotImplementedException();
#endif
        }


        public static bool Exists(string path)
        {
#if NETFX_CORE
            var t = ExistsAsync(path.Replace('/', '\\'));
            t.Wait();
            if (t.IsCompleted)
                return t.Result;
            else
                return false;
#else
            throw new NotImplementedException();
#endif
        }


#if NETFX_CORE

        /// <summary>
        /// Creates a folder in local iso storage
        /// </summary>
        private static async Task<bool> CreateDirectoryAsync(string folderName)
        {

            try
            {
                await ApplicationData.Current.LocalFolder.CreateFolderAsync(folderName, CreationCollisionOption.ReplaceExisting);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private static async Task<bool> ExistsAsync(string path)
        {
            try
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(path);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static async Task<string[]> GetFilesAsync(string path)
        {
            try
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(path);
                var files = await folder.GetFilesAsync();
                var result = new string[files.Count];

                for (int i = 0; i < files.Count; i++)
                    result[i] = Path.Combine(path, files[i].Name);
                return result;
            }
            catch
            {
                return null;
            }
        }

#endif
    }
}