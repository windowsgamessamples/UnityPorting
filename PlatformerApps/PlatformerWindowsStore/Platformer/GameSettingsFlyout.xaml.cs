using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769
using UnityPlayer;

namespace Template
{
    public sealed partial class GameSettingsFlyout : SettingsFlyout
    {
        public GameSettingsFlyout()
        {
            this.InitializeComponent();
        }

        public void SetSwitchToggleState()
        {
            AppCallbacks.Instance.InvokeOnUIThread(async () =>
            {
                SoundToggleSwitch.IsOn = WindowsGateway.IsSoundEnabled();
            }, false);
            
        }

        private void SoundToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            AppCallbacks.Instance.InvokeOnUIThread(async () =>
            {
                WindowsGateway.EnableSound(SoundToggleSwitch.IsOn);
            }, false);
        }
    }
}
