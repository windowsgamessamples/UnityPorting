using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace MyPlugin.Legacy.System.IO
{
    internal class EncryptedStreamWriter : StreamWriter
    {
        public EncryptedStreamWriter(Stream str)
            : base(str)
        {
        }

        public override void WriteLine(string value)
        {
            try
            {
                value = EncryptionProvider.Encrypt(value, CurrentApp.AppId.ToString());

                base.WriteLine(value);
            }
            catch { }
        }
    }
}