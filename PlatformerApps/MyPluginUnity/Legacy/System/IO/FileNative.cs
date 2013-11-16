using System;
using System.IO;

#if NETFX_CORE
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
#endif

namespace MyPlugin.Legacy.System.IO
{

    public static class FileNative
    {

        public static bool Exists(string path)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = ExistsAsync(path);
            try
            {
                thread.Wait();

                if (thread.IsCompleted)
                    return thread.Result;
                else
                    return false;
            }
            catch
            {
                return false;
            }
#else
            throw new NotImplementedException();
#endif
        }

        public static string ReadAllText(string path)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = PathIO.ReadTextAsync(path).AsTask();
            thread.Wait();

            if (thread.IsCompleted)
                return thread.Result;

            throw thread.Exception;
#else
            throw new NotImplementedException();
#endif
        }

        public static byte[] ReadAllBytes(string path)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = ReadAllBytesAsync(path);
            thread.Wait();

            if (thread.IsCompleted)
                return thread.Result;

            throw thread.Exception;
#else
            throw new NotImplementedException();
#endif
        }

        public static void WriteAllBytes(string path, byte[] data)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = PathIO.WriteBytesAsync(path, data).AsTask();
            thread.Wait();
#else
            throw new NotImplementedException();
#endif
        }

        public static void WriteAllText(string path, string data)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = PathIO.WriteTextAsync(path, data).AsTask();
            thread.Wait();
#else
            throw new NotImplementedException();
#endif
        }

        public static void Move(string source, string destination)
        {
#if NETFX_CORE
            source = FixPath(source);
            destination = FixPath(destination);

            var thread = MoveAsync(source, destination);
            thread.Wait();
#else
            throw new NotImplementedException();
#endif
        }

        public static void Delete(string path)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = DeleteAsync(path);
            thread.Wait();
#else
            throw new NotImplementedException();
#endif
        }

        public static Stream Create(string path)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = CreateAsync(path);
            thread.Wait();

            if (thread.IsCompleted)
                return thread.Result;

            throw thread.Exception;
#else
            throw new NotImplementedException();
#endif
        }

        public static StreamWriter CreateText(string path)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = CreateTextAsync(path);
            thread.Wait();

            if (thread.IsCompleted)
                return thread.Result;

            throw thread.Exception;
#else
            throw new NotImplementedException();
#endif
        }

        public static Stream Open(string path)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = OpenAsync(path);
            thread.Wait();

            if (thread.IsCompleted)
                return thread.Result;

            throw thread.Exception;
#else
            throw new NotImplementedException();
#endif
        }

        public static StreamReader OpenText(string path)
        {
#if NETFX_CORE
            path = FixPath(path);
            var thread = OpenTextAsync(path);
            thread.Wait();

            if (thread.IsCompleted)
                return thread.Result;

            throw thread.Exception;
#else
            throw new NotImplementedException();
#endif
        }


        public static string StripLocalFolder(string path)
        {
#if NETFX_CORE
            path = path.Replace('/', '\\');
            var localPath = ApplicationData.Current.LocalFolder.Path.ToLower();

            if (path.ToLower().StartsWith(localPath))
                path = path.Substring(localPath.Length + 1);
            return path;
#else
            throw new NotImplementedException();
#endif
        }

#if NETFX_CORE


        private static EncryptedStreamReader OpenEncryptedText(string path)
        {
            path = FixPath(path);
            if (Exists(path))
            {
                var thread = OpenEncryptedTextAsync(path);
                thread.Wait();

                if (thread.IsCompleted)
                    return thread.Result;

                throw thread.Exception;
            }
            else
            {
                return null;
            }
        }

        private static EncryptedStreamWriter CreateEncryptedText(string path)
        {

            path = FixPath(path);
            var thread = CreateEncryptedTextAsync(path);
            thread.Wait();

            if (thread.IsCompleted)
                return thread.Result;

            throw thread.Exception;
        }

        private static async Task MoveAsync(string source, string destination)
        {
            var file = await StorageFile.GetFileFromPathAsync(source);
            var destinatinoFolder = await StorageFolder.GetFolderFromPathAsync(destination);
            if (file != null && destinatinoFolder != null)
            {
                await file.MoveAsync(destinatinoFolder);
            }
        }

        private static async Task<Stream> OpenAsync(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            var stream = await file.OpenStreamForReadAsync();
            return stream;
        }

        private static async Task<StreamReader> OpenTextAsync(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            var stream = await file.OpenStreamForReadAsync();
            return new StreamReader(stream);
        }

        private static async Task<EncryptedStreamReader> OpenEncryptedTextAsync(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);

            var stream = await file.OpenStreamForReadAsync();
            return new EncryptedStreamReader(stream);
        }

        private static string FixPath(string path)
        {
            return path.Replace('/', '\\');
        }

        /* Copy ********************************************************************/

        private static async Task CopyAsync(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!overwrite && Exists(destFileName))
                return;

            var sourceDirName = Path.GetDirectoryName(sourceFileName);
            var destDirName = Path.GetDirectoryName(destFileName);

            var sourceDir = await StorageFolder.GetFolderFromPathAsync(sourceDirName);
            var destDir = await StorageFolder.GetFolderFromPathAsync(destDirName);

            var sourceFile = await sourceDir.GetFileAsync(Path.GetFileName(sourceFileName));
            var destFile = await destDir.CreateFileAsync(Path.GetFileName(destFileName), CreationCollisionOption.ReplaceExisting);

            await sourceFile.CopyAndReplaceAsync(destFile);

        }

        private static async Task<StreamWriter> CreateTextAsync(string path)
        {
            var str = await CreateAsync(path);
            return new StreamWriter(str);
        }


        private async static void CopyInitialisationFile(string fileName)
        {
            var db = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Integration/Initialization/" + fileName, UriKind.RelativeOrAbsolute));

            string localFolder = ApplicationData.Current.LocalFolder.Path;
            await db.CopyAsync(ApplicationData.Current.LocalFolder, fileName, NameCollisionOption.ReplaceExisting);
        }

        private static async Task<bool> ExistsAsync(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            return file != null;
        }

        private static async Task<byte[]> ReadAllBytesAsync(string path)
        {
            var buffer = await PathIO.ReadBufferAsync(path);
            using (var dr = DataReader.FromBuffer(buffer))
            {
                await dr.LoadAsync(buffer.Length);
                byte[] data = new byte[buffer.Length];
                dr.ReadBytes(data);
                return data;
            }
        }
        private static async Task DeleteAsync(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            if (file != null)
                await file.DeleteAsync();
        }

        private static async Task<Stream> CreateAsync(string path)
        {
            var dirName = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);

            var dir = await StorageFolder.GetFolderFromPathAsync(dirName);
            var file = await dir.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            return await file.OpenStreamForWriteAsync();
        }

        private static async Task<EncryptedStreamWriter> CreateEncryptedTextAsync(string path)
        {
            var str = await CreateAsync(path);
            return new EncryptedStreamWriter(str);
        }

#endif

    }

}
