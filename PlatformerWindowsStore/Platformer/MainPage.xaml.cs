using System;
using System.Threading.Tasks;
using UnityPlayer;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.ApplicationSettings;
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
        private static SettingsPane settingsPane;


        public MainPage(SplashScreen splashScreen)
        {
            this.InitializeComponent();

            // initialize extended splash
            splash = splashScreen;
            SetExtendedSplashBackgroundColor();

            // ensure we are aware of app window being resized
            OnResize();
            Window.Current.SizeChanged += onResizeHandler = new WindowSizeChangedEventHandler((o, e) => OnResize(e));

            // ensure we listen to when unity tells us game is ready
            WindowsGateway.UnityLoaded = OnUnityLoaded;

            // create extended splash timer
            extendedSplashTimer = new DispatcherTimer();
            extendedSplashTimer.Interval = TimeSpan.FromMilliseconds(100);
            extendedSplashTimer.Tick += ExtendedSplashTimer_Tick;
            extendedSplashTimer.Start();

            // configure settings charm
            settingsPane = SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += OnSettingsCommandsRequested;

            // configure share charm
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            WindowsGateway.ShowShareUI = OnShowShareUI;
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
                await Task.Delay(250); // force a little delay so that user can see progress bar maxing out very briefly
                RemoveExtendedSplash();
            }
        }

        /// <summary>
        /// Unity has loaded and the game is playable 
        /// </summary>
        private async void OnUnityLoaded()
        {
            await Task.Delay(3000); // faked delay as sample game loads very quickly, remove in a release game!
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
                AppCallbacks.Instance.InvokeOnAppThread(() =>
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

        /// <summary>
        /// Show the Share Charm
        /// </summary>
        static void OnShowShareUI()
        {
            AppCallbacks.Instance.InvokeOnUIThread(() =>
            {
                DataTransferManager.ShowShareUI();
            }, false);
        }

        static void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            // TODO retrieve the player's score from Unity!
            var score = 500;

            if (score <= 0)
            {
                request.Data.Properties.Title = "Platformer";
            }
            else
            {
                request.Data.Properties.Title = String.Format("Platformer High Score of {0}!", score);
                request.Data.Properties.Description = "Check out my hi score on Platformer!";
            }

            try
            {
                request.Data.SetUri(Windows.ApplicationModel.Store.CurrentApp.LinkUri);
            }
            catch // exception will be thrown if app not published, manually set a url for testing
            {
                request.Data.SetUri(new Uri("http://www.mygameurl.com"));
            }
        }

        private static void OnSettingsCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("privacy", "Privacy Policy", h => OnViewPrivacyPolicy()));
            args.Request.ApplicationCommands.Add(new SettingsCommand("termsofuse", "Terms of Use", h => OnViewTermsOfUse()));
        }

        private static void OnViewTermsOfUse()
        {
            AppCallbacks.Instance.InvokeOnUIThread(async () =>
            {
                await Launcher.LaunchUriAsync(new Uri("http://www.myplatformergame.com/corp/tos.html", UriKind.Absolute));
            }, false);
        }

        private static void OnViewPrivacyPolicy()
        {
            AppCallbacks.Instance.InvokeOnUIThread(async () =>
            {
                await Launcher.LaunchUriAsync(new Uri("http://www.myplatformergame.com/corp/privacy.html", UriKind.Absolute));
            }, false);
        }

        public SwapChainBackgroundPanel GetSwapChainBackgroundPanel()
        {
            return DXSwapChainBackgroundPanel;
        }

    }
}
