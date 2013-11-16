#if UNITY_METRO && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyPlugin.WACK.System.IO;

namespace System.IO
{
    public class File
    {

        public static void Move(string sourceFileName, string destFileName)
        {
            FileNative.Move(sourceFileName, destFileName);
        }

        public static bool Exists(string path)
        {
            return FileNative.Exists(path);
        }

        public static string ReadAllText(string path)
        {
            return FileNative.ReadAllText(path);
        }

        public static byte[] ReadAllBytes(string path)
        {
            return FileNative.ReadAllBytes(path);
        }

        public static void WriteAllBytes(string path, byte[] data)
        {
            FileNative.WriteAllBytes(path, data);
        }

        public static void WriteAllText(string path, string data)
        {
            FileNative.WriteAllText(path, data);
        }

        public static void Delete(string path)
        {
            FileNative.Delete(path);
        }

        public static Stream Open(string path)
        {
            return FileNative.Open(path);
        }

        public static Stream Create(string path)
        {
            return FileNative.Create(path);
        }

        public static StreamWriter CreateText(string path)
        {
            return FileNative.CreateText(path);
        }

        public static StreamReader OpenText(string path)
        {
            return FileNative.OpenText(path);
        }
    }
}


#endif
