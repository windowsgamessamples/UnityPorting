using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Template
{
    public sealed partial class MainPage : Page
    {
        private SplashScreen splash;
        private Rect splashImageRect;
        private WindowSizeChangedEventHandler onResizeHandler;
        private DispatcherTimer extendedSplashTimer;
        private bool isUnityLoaded;

        public MainPage(SplashScreen splashScreen)
        {
            this.InitializeComponent();

            // initialize extended splash
            splash = splashScreen;
            SetExtendedSplashBackgroundColor();

            // ensure we are aware of app window being resuzed
            OnResize();
            Window.Current.SizeChanged += onResizeHandler = new WindowSizeChangedEventHandler((o, e) => OnResize(e));

            // ensure we listen to when unity tells us game is ready
            WindowsGateway.UnityLoaded = OnUnityLoaded;

            // create extended splash timer
            extendedSplashTimer = new DispatcherTimer();
            extendedSplashTimer.Interval = TimeSpan.FromMilliseconds(100);
            extendedSplashTimer.Tick += ExtendedSplashTimer_Tick;
            extendedSplashTimer.Start();
        }

        /// <summary>
        /// Control the extended splash experience
        /// </summary>
        async void ExtendedSplashTimer_Tick(object sender, object e)
        {
            var increment = extendedSplashTimer.Interval.TotalMilliseconds;
            if (!isUnityLoaded && SplashProgress.Value <= (SplashProgress.Maximum - increment))
            {
                SplashProgress.Value += increment;
            }
            else
            {
                SplashProgress.Value = SplashProgress.Maximum;
                await Task.Delay(250);
                RemoveExtendedSplash();
            }
        }

        /// <summary>
        /// Unity has loaded and the game is playable 
        /// </summary>
        private async void OnUnityLoaded()
        {
            await Task.Delay(3000); // faked delay as sample game loads very quickly!
            isUnityLoaded = true;
        }

        /// <summary>
        /// Respond to window resizing
        /// </summary>
        private void OnResize(WindowSizeChangedEventArgs args = null)
        {
            if (splash != null)
            {
                // extended splash is still visible, game not loaded
                splashImageRect = splash.ImageLocation;
                ExtendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
                ExtendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
                ExtendedSplashImage.Height = splashImageRect.Height;
                ExtendedSplashImage.Width = splashImageRect.Width;
            }
            else if (args != null)
            {
                // Game has loaded, tell Unity engine that the window size has changed
                var height = args.Size.Height;
                var width = args.Size.Width;
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    WindowsGateway.WindowSizeChanged(height, width);
                }, false);
            }
        }

        /// <summary>
        /// Set the extended splash background color based on app manifest
        /// </summary>
        private async void SetExtendedSplashBackgroundColor()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///AppxManifest.xml"));
                string manifest = await FileIO.ReadTextAsync(file);
                int idx = manifest.IndexOf("SplashScreen");
                manifest = manifest.Substring(idx);
                idx = manifest.IndexOf("BackgroundColor");
                if (idx < 0)  // background is optional
                    return;
                manifest = manifest.Substring(idx);
                idx = manifest.IndexOf("\"");
                manifest = manifest.Substring(idx + 2); // also remove quote and # char after it
                idx = manifest.IndexOf("\"");
                manifest = manifest.Substring(0, idx);
                int value = Convert.ToInt32(manifest, 16) & 0x00FFFFFF;
                byte r = (byte)(value >> 16);
                byte g = (byte)((value & 0x0000FF00) >> 8);
                byte b = (byte)(value & 0x000000FF);

                await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate()
                {
                    ExtendedSplashGrid.Background = new SolidColorBrush(Color.FromArgb(0xFF, r, g, b));
                });
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Remove the extended splash 
        /// </summary>
        public void RemoveExtendedSplash()
        {
            if (extendedSplashTimer != null)
            {
                extendedSplashTimer.Stop();
            }
            if (DXSwapChainBackgroundPanel.Children.Count > 0)
            {
                DXSwapChainBackgroundPanel.Children.Remove(ExtendedSplashGrid);
                splash = null;
            }
        }

        public SwapChainBackgroundPanel GetSwapChainBackgroundPanel()
        {
            return DXSwapChainBackgroundPanel;
        }

    }
}
