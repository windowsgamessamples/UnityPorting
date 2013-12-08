
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.Graphics.Display;
using Windows.ApplicationModel;
#elif WINDOWS_PHONE
using Microsoft.Phone.Tasks;
using System.Xml.Linq;
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
        /// Returns the application package version 
        /// </summary>
        /// <returns></returns>
        public string GetAppVersion()
        {
#if NETFX_CORE
            var major = Package.Current.Id.Version.Major;
            var minor = Package.Current.Id.Version.Minor.ToString();
            var revision = Package.Current.Id.Version.Revision.ToString();
            var build = Package.Current.Id.Version.Build.ToString();
            var version = String.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
            return version;
#elif WINDOWS_PHONE
            return XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
#else
            return String.Empty;
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
            Dispatcher.InvokeOnUIThread(() =>
            {
                Windows.Graphics.Display.DisplayProperties.AutoRotationPreferences = (Windows.Graphics.Display.DisplayOrientations)value; 
            });
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
                Dispatcher.InvokeOnAppThread(() =>
                {
                    eh(this, null);
                });
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
