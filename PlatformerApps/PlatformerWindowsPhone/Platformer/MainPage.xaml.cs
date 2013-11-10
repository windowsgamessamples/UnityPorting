using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Windows.ApplicationModel.Activation;
using Microsoft.Phone.Controls;
using Windows.Foundation;
using Windows.Devices.Geolocation;

using UnityApp = UnityPlayer.UnityApp;
using UnityBridge = WinRTBridge.WinRTBridge;

namespace Platformer
{
	public partial class MainPage : PhoneApplicationPage
	{
	    private SplashScreen _splash;
		private bool _unityStartedLoading;
		private bool _useLocation;
        private bool _isUnityLoaded;
        private DispatcherTimer _extendedSplashTimer;


		// Constructor
		public MainPage()
		{
			var bridge = new UnityBridge();
			UnityApp.SetBridge(bridge);
			InitializeComponent();
			bridge.Control = DrawingSurfaceBackground;

            _extendedSplashTimer = new DispatcherTimer();
            _extendedSplashTimer.Interval = TimeSpan.FromMilliseconds(50);
            _extendedSplashTimer.Tick += ExtendedSplashTimer_Tick;
            _extendedSplashTimer.Start();

            // ensure we listen to when unity tells us game is ready
            WindowsGateway.UnityLoaded = OnUnityLoaded;

		}

        async void ExtendedSplashTimer_Tick(object sender, EventArgs e)
        {
            var increment = _extendedSplashTimer.Interval.TotalMilliseconds * 10;
            if (!_isUnityLoaded && SplashProgress.Value <= (SplashProgress.Maximum - increment))
                SplashProgress.Value += increment;
            else
            {
                SplashProgress.Value = SplashProgress.Maximum;
                await Task.Delay(150);
                RemoveExtendedSplash();
            }
        }

	    private async void OnUnityLoaded()
	    {
	        await Task.Delay(0);
	        _isUnityLoaded = true;
	    }

	    private void RemoveExtendedSplash()
	    {
            if(_extendedSplashTimer != null)
                _extendedSplashTimer.Stop();

	        if (DrawingSurfaceBackground.Children.Count > 0)
	            DrawingSurfaceBackground.Children.Remove(ExtendedSplashGrid);
	    }

	    private void DrawingSurfaceBackground_Loaded(object sender, RoutedEventArgs e)
		{
			if (!_unityStartedLoading)
			{
				_unityStartedLoading = true;

				UnityApp.SetLoadedCallback(() => { Dispatcher.BeginInvoke(Unity_Loaded); });

				var content = Application.Current.Host.Content;
				var width = (int)Math.Floor(content.ActualWidth * content.ScaleFactor / 100.0 + 0.5);
				var height = (int)Math.Floor(content.ActualHeight * content.ScaleFactor / 100.0 + 0.5);

				UnityApp.SetNativeResolution(width, height);
				UnityApp.SetRenderResolution(width, height);
				UnityApp.SetOrientation((int)Orientation);

				DrawingSurfaceBackground.SetBackgroundContentProvider(UnityApp.GetBackgroundContentProvider());
				DrawingSurfaceBackground.SetBackgroundManipulationHandler(UnityApp.GetManipulationHandler());
			}
		}

        

		private void Unity_Loaded()
		{
			SetupGeolocator();
		}

		private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
		{
			e.Cancel = UnityApp.BackButtonPressed();
		}

		private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
		{
			UnityApp.SetOrientation((int)e.Orientation);
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!UnityApp.IsLocationEnabled())
                return;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
                _useLocation = (bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"];
            else
            {
                MessageBoxResult result = MessageBox.Show("Can this application use your location?",
                    "Location Services", MessageBoxButton.OKCancel);
                _useLocation = result == MessageBoxResult.OK;
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = _useLocation;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

		private void SetupGeolocator()
        {
            if (!_useLocation)
                return;

            try
            {
				UnityApp.EnableLocationService(true);
                Geolocator geolocator = new Geolocator();
				geolocator.ReportInterval = 5000;
                IAsyncOperation<Geoposition> op = geolocator.GetGeopositionAsync();
                op.Completed += (asyncInfo, asyncStatus) =>
                    {
                        if (asyncStatus == AsyncStatus.Completed)
                        {
                            Geoposition geoposition = asyncInfo.GetResults();
                            UnityApp.SetupGeolocator(geolocator, geoposition);
                        }
                        else
                            UnityApp.SetupGeolocator(null, null);
                    };
            }
            catch (Exception)
            {
                UnityApp.SetupGeolocator(null, null);
            }
        }
	}
}