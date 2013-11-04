using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Windows;
using UnityPlayer;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Template
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SplashScreen splash;
        private Rect splashImageRect;
        private WindowSizeChangedEventHandler onResizeHandler;

        private DispatcherTimer timer;

        public MainPage(SplashScreen splashScreen)
        {
            this.InitializeComponent();

            splash = splashScreen;
            GetSplashBackgroundColor();
            OnResize();
            Window.Current.SizeChanged += onResizeHandler = new WindowSizeChangedEventHandler((o, e) => OnResize(e));

            // TODO - need to have unity tell us when the scene is actually loaded and ready. AppCallbacks.Initialized happens too early in most cases.

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void Instance_Initialized()
        {
            RemoveSplashScreen();
        }

        void timer_Tick(object sender, object e)
        {
            var increment = timer.Interval.TotalMilliseconds;
            if (SplashProgress.Value <= (SplashProgress.Maximum - increment))
            {
                SplashProgress.Value += increment;
            }
            else
            {
                SplashProgress.Value = SplashProgress.Maximum;
                RemoveSplashScreen();
            }
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            splash = (SplashScreen)e.Parameter;
            OnResize();
            timer.Start();
        }

        private void OnResize(WindowSizeChangedEventArgs args)
        {
            if (splash != null)
            {
                splashImageRect = splash.ImageLocation;
                PositionImage();
            }
            else
            {
                // Tell Unity engine that the window size has changed
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                    {
                        WindowsGateway.WindowSizeChanged(args.Size.Height, args.Size.Width);
                    }, false);
            }
        }

        private void PositionImage()
        {
            ExtendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            ExtendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            ExtendedSplashImage.Height = splashImageRect.Height;
            ExtendedSplashImage.Width = splashImageRect.Width;
        }

        private async void GetSplashBackgroundColor()
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
                byte r = (byte) (value >> 16);
                byte g = (byte) ((value & 0x0000FF00) >> 8);
                byte b = (byte) (value & 0x000000FF);

                CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate()
                    {
                        ExtendedSplashGrid.Background = new SolidColorBrush(Color.FromArgb(0xFF, r, g, b));
                    });
            }
            catch (Exception)
            {}
        }

        public SwapChainBackgroundPanel GetSwapChainBackgroundPanel()
		{
            return DXSwapChainBackgroundPanel;
		}

        public void RemoveSplashScreen()
        {
            if (timer != null)
            {
                timer.Stop();
            }
            if (DXSwapChainBackgroundPanel.Children.Count > 0)
            { 
                DXSwapChainBackgroundPanel.Children.Remove(ExtendedSplashGrid);
            }
            if (onResizeHandler != null)
            {
                Window.Current.SizeChanged -= onResizeHandler;
                onResizeHandler = null;
            }
        }
    }
}
