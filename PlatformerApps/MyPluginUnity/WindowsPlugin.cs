
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.Graphics.Display;
#elif WINDOWS_PHONE
using Microsoft.Phone.Tasks;
#endif

namespace MyPlugin
{

    /// <summary>
    /// Allows for some common windows integration scenarops
    /// </summary>
    public class WindowsPlugin : IDisposable
    {

        private static WindowsPlugin _instance;
        private static readonly object _sync = new object();

        public static WindowsPlugin Instance
        {
            get
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new WindowsPlugin();
                }
                return _instance;
            }
        }

        public WindowsPlugin()
        {
#if NETFX_CORE 
            Dispatcher.InvokeOnUIThread(() =>
            {
                DisplayInformation.GetForCurrentView().OrientationChanged += WindowsPlugin_OrientationChanged;
            });
#endif 
        }

        /// <summary>
        /// Will allow Unity to respond to orientation changes
        /// </summary>
        public EventHandler OrientationChanged; 

        /// <summary>
        /// Allows Unity to set auto rotation preferences
        /// </summary>
        void SetOrientationPreferences ( int value )
        {
#if NETFX_CORE 
            Windows.Graphics.Display.DisplayProperties.AutoRotationPreferences =
                (Windows.Graphics.Display.DisplayOrientations)value; 
#endif 
        }

        public void Dispose()
        {
#if NETFX_CORE 
            Dispatcher.InvokeOnUIThread(() =>
            {
                DisplayInformation.GetForCurrentView().OrientationChanged -= WindowsPlugin_OrientationChanged;
            });
#endif 
        }

#if NETFX_CORE 
        void WindowsPlugin_OrientationChanged(DisplayInformation sender, object args)
        {
            var eh = OrientationChanged;
            if (eh != null)
            {
              eh(this, null);
            }
        }
#endif

        /// <summary>
        /// Show the Share UI
        /// </summary>
        public void ShowShareUI()
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
