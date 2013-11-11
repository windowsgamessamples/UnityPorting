#if !UNITY_EDITOR && UNITY_METRO
namespace System.IO
{
    using System;

    public static class Directory
    {
        public static Func<string, string[]> DoGetFiles;
        public static Func<string, bool> DoExists;
        public static Func<string, string, string[]> DoGetFilesWithSearchPattern;
        public static Func<string, bool> DoCreateDirectory;
        public static Func<string> DoGetAppDir; 

        public static string[] GetFiles(string path)
        {
            return DoGetFiles(path);
        }

        public static bool Exists(string path)
        {
            return DoExists(path);
        }

        public static bool CreateDirectory(string path)
        {
            return DoCreateDirectory(path);
        }

        //public static string[] GetFiles(string path, string searchPattern)
        //{
        //    return DoGetFilesWithSearchPattern(path, searchPattern);
        //}

        public static string GetCurrentDirectory()
        {
            return DoGetAppDir();
        }
    }
}
#endif