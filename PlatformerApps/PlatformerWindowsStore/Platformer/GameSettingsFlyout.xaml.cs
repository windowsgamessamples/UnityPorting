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
            AppCallbacks.Instance.InvokeOnAppThread(() =>
            {
                var isSoundEnabled = GameManager.Instance.IsSoundEnabled();
                AppCallbacks.Instance.InvokeOnUIThread(() =>
                {
                    SoundToggleSwitch.IsOn = isSoundEnabled;
                }, false);
            }, false);  
        }

        private void SoundToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            AppCallbacks.Instance.InvokeOnUIThread(() =>
            {
                var isSoundEnabled = SoundToggleSwitch.IsOn;
                AppCallbacks.Instance.InvokeOnAppThread(() =>
                {
                    GameManager.Instance.EnableSound(isSoundEnabled);
                }, false);
            }, false);  
        }
    }
}
