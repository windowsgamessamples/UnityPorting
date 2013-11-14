
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
#elif WINDOWS_PHONE
using Microsoft.Phone.Tasks;
#endif

namespace MyPlugin
{

    public class WindowsPlugin
    {

        /// <summary>
        /// Show the Share UI
        /// </summary>
        public static void ShowShareUI()
        {
#if NETFX_CORE
            Dispatcher.InvokeOnUIThread(() => DataTransferManager.ShowShareUI());
#elif WINDOWS_PHONE
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.Title = "Great Platformer Game!";
            shareLinkTask.LinkUri = new Uri("http://code.msdn.com/wpapps", UriKind.Absolute);
            shareLinkTask.Message = "Sharing the app for windows phone example!.";
            shareLinkTask.Show();
#endif
        }


    }
}
