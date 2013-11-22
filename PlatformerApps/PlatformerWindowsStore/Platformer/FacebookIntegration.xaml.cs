using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyPlugin.Facebook;
using UnityPlayer;

namespace Template
{
    /// <summary>
    /// Supports facebook integratino 
    /// </summary>
    public sealed partial class FacebookIntegration : UserControl
    {
        public FacebookIntegration()
        {
            this.InitializeComponent();

            // facebook overlay integration
            WebPopup.Opened += PopupOpened;
            WebPopup.Closed += PopupClosed;
            MyPlugin.Facebook.FacebookGateway.Instance.Initialise(FacebookOverlay);
            MyPlugin.Facebook.FacebookGateway.Instance.StateChanged += FacebookStateChanged;
        }

        private void PopupClosed(object sender, object e)
        {
            MyPlugin.Facebook.FacebookGateway.Instance.Cancel();
            WebOverlay.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            // Enable Unity input when popup has closed
            AppCallbacks.Instance.UnitySetInput(true);
        }

        private void PopupOpened(object sender, object e)
        {                        
            WebOverlay.Visibility = Windows.UI.Xaml.Visibility.Visible;

            // Disable Unity input when popup has opened, this prevents known keypress issues 
            AppCallbacks.Instance.UnitySetInput(false);
        }

        private void FacebookStateChanged(FacebookRequest request, NavigationState state)
        {
            switch (state)
            {
                case NavigationState.Done:
                case NavigationState.Error:
                    //WebOverlay.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    WebPopup.IsOpen = false;
                    FacebookOverlay.NavigateToString("");
                    break;

                case NavigationState.Navigating:
                    switch (request)
                    {
                        case FacebookRequest.Logout:
                            FacebookOverlay.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            CancelButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                            break;

                        case FacebookRequest.Login:
                        case FacebookRequest.InviteRequest:
                            FacebookOverlay.Visibility = Windows.UI.Xaml.Visibility.Visible;
                            CancelButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                            break;
                    }
                    //WebOverlay.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    WebPopup.IsOpen = true;
                    break;
            }
        }

        private void CancelWeb(object sender, RoutedEventArgs e)
        {
            MyPlugin.Facebook.FacebookGateway.Instance.Cancel();
        }

    }
}
