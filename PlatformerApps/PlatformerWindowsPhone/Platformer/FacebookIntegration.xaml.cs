using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyPlugin.Facebook;

namespace Platformer
{
    public partial class FacebookIntegration : UserControl
    {
        public FacebookIntegration()
        {
            this.InitializeComponent();

            // facebook overlay integration
            //WebPopup.Opened += PopupOpened;
            //WebPopup.Closed += PopupClosed;
            MyPlugin.Facebook.FacebookGateway.Instance.Initialise(FacebookOverlay);
            MyPlugin.Facebook.FacebookGateway.Instance.StateChanged += FacebookStateChanged;
        }

        private void PopupClosed(object sender, object e)
        {
            MyPlugin.Facebook.FacebookGateway.Instance.Cancel();
            WebOverlay.Visibility = Visibility.Collapsed;
        }

        private void PopupOpened(object sender, object e)
        {
            WebOverlay.Visibility = Visibility.Visible;
        }

        private void FacebookStateChanged(FacebookRequest request, NavigationState state)
        {
            switch (state)
            {
                case NavigationState.Done:
                    MainPage.Current.FacebookGrid.Visibility = Visibility.Collapsed;
                    break;
                case NavigationState.Error:
                    //WebOverlay.Visibility = Visibility.Collapsed;
                    //WebPopup.IsOpen = false;
                    FacebookOverlay.NavigateToString("");
                    break;

                case NavigationState.Navigating:
                    switch (request)
                    {
                        case FacebookRequest.Logout:
                            //FacebookOverlay.Visibility = Visibility.Collapsed;
                            //CancelButton.Visibility = Visibility.Collapsed;
                            MainPage.Current.FacebookGrid.Visibility = Visibility.Collapsed;
                            break;

                        case FacebookRequest.Login:
                            //WebOverlay.Visibility = Visibility.Visible;
                            //FacebookOverlay.Visibility = Visibility.Visible;
                            //var mainPage = MainPage.Current;
                            MainPage.Current.FacebookGrid.Visibility = Visibility.Visible;
                            //mainPage.FacebookIntegrationControl.Visibility = Visibility.Visible;
                            break;
                        case FacebookRequest.InviteRequest:
                            //FacebookOverlay.Visibility = Visibility.Visible;
                            MainPage.Current.FacebookGrid.Visibility = Visibility.Visible;
                            //CancelButton.Visibility = Visibility.Visible;
                            break;
                    }
                    //WebOverlay.Visibility = Visibility.Visible;
                    //WebPopup.IsOpen = true;
                    break;
            }
        }

        private void CancelWeb(object sender, RoutedEventArgs e)
        {
            MyPlugin.Facebook.FacebookGateway.Instance.Cancel();
        }
    }
}
