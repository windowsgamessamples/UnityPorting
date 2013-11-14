
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

#if NETFX_CORE

        // needs to be set via the app so we can invoke onto UI Thread (see App.xaml.cs)
        public static CoreDispatcher UIDispatcher 
        {get;set;}

#endif

        /// <summary>
        /// Show the Share UI
        /// </summary>
        
#if UNITY_EDITOR

        public static void ShowShareUI()
        {
            throw new NotImplementedException();
        }

#elif NETFX_CORE

        public static async void ShowShareUI()
        {
            await UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DataTransferManager.ShowShareUI();
            });
        }

#elif WINDOWS_PHONE

        public static void ShowShareUI()
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.Title = "Great Platformer Game!";
            shareLinkTask.LinkUri = new Uri("http://code.msdn.com/wpapps", UriKind.Absolute);
            shareLinkTask.Message = "Sharing the app for windows phone example!.";
            shareLinkTask.Show();
        }
            
#endif

    }
}
