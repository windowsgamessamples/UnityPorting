using MyPlugin;
using System;
#if NETFX_CORE
using Windows.UI.Core;
#endif

namespace MyPlugin.Facebook
{

    public static class FacebookPlugin
    {

#if !NETFX_CORE
        public static void InviteFriend(string friendName)
        {
#else
        public static async void InviteFriend(string friendName)
        {
            await WindowsPlugin.CurrentDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Facebook.Instance.InviteFriendsAsync(friendName);
            });
#endif
        }

#if !NETFX_CORE
        public static void Logout(Action callback)
        {
#else
        public static async void Logout(Action callback)
        {
            await WindowsPlugin.CurrentDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var task = Facebook.Instance.LogoutAsync();
                if (callback != null)
                {
                    task.ContinueWith(t => callback()); // TODO Invoke On Unity App Thread
                }
            });
#endif
        }


#if !NETFX_CORE
        public static void Login(Action<string> callback)
        {
#else
        public static async void Login(Action<string> callback)
        {
            await WindowsPlugin.CurrentDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var task = Facebook.Instance.LoginAsync();
                task.ContinueWith(
                    t =>
                    {
                        if (t.Result == NavigationState.Done && callback != null)
                        {
                            callback(Facebook.Instance.AccessToken); // TODO Invoke on App Thread
                        }
                    });
            });
#endif
        }

    }

}
