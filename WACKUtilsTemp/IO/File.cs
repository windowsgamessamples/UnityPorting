#if !UNITY_EDITOR && UNITY_METRO    
namespace System.IO
{
    public class File
    {
        public static Func<string, bool> DoFileExists;
        public static Func<string, string> DoReadAllText;
        public static Func<string, byte[]> DoReadAllBytes;
        public static Action<string, byte[]> DoWriteAllBytes;
        public static Action<string, string> DoWriteAllText;
        public static Action<string> DoDelete;
        public static Func<string, Stream> DoCreate;
        public static Func<string, Stream> DoOpen;
        //public static Func<string, Stream> DoOpenRead;
        public static Func<string, StreamWriter> DoCreateText;
        public static Func<string, StreamReader> DoOpenText;
        public static Action<string, string> DoMove;
        public static Action<string, string, bool> DoCopy;
        public static Action<string> DoCopyInitialisationFile;
        public static Func<string, string[]> DoReadAllLines;

        public static void Move(string sourceFileName, string destFileName)
        {
            DoMove(sourceFileName, destFileName);
        }

        public static void CopyInitialisationFile(string fileName)   
        {
            DoCopyInitialisationFile(fileName);
        }

        public static void Copy(string sourceFileName, string destFileName, bool overWrite)
        {
            DoCopy(sourceFileName, destFileName, overWrite);
        }

        public static bool Exists(string path)
        {
            return DoFileExists(path);
        }

        public static string ReadAllText(string path)
        {
            return DoReadAllText(path);
        }

        public static string[] ReadAllLines(string path)
        {
            return DoReadAllLines(path);
        }

        public static byte[] ReadAllBytes(string path)
        {
            return DoReadAllBytes(path);
        }

        public static void WriteAllBytes(string path, byte[] data)
        {
            DoWriteAllBytes(path, data);
        }

        public static void WriteAllText(string path, string data)
        {
            DoWriteAllText(path, data);
        }

        public static void Delete(string path)
        {
            DoDelete(path);
        }

        public static Stream Open(string path)
        {
            return DoOpen(path);
        }

        //public static Stream OpenRead(string path)
        //{
        //    return DoOpenRead(path);
        //}

        public static Stream Create(string path)
        {
            return DoCreate(path);
        }

        public static StreamWriter CreateText(string path)
        {
            return DoCreateText(path);
        }

        public static StreamReader OpenText(string path)
        {
            return DoOpenText(path);
        }
    }
}
#endif