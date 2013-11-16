#if UNITY_METRO && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyPlugin.Legacy.System.IO;

namespace System.IO
{
    public class Directory
    {

        public static string[] GetFiles(string path)
        {
            return DirectoryNative.GetFiles(path);
        }

        public static bool Exists(string path)
        {
            return DirectoryNative.Exists(path);
        }

        public static bool CreateDirectory(string path)
        {
            return DirectoryNative.CreateDirectory(path);
        }

        public static string GetCurrentDirectory()
        {
            return DirectoryNative.GetAppDirectory();
        }

    }
}

#endif