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
            await Dispatcher.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await FacebookGateway.Instance.InviteFriendsAsync(friendName);
            });
#endif
        }

#if !NETFX_CORE
        public static void Logout(Action callback)
        {
#else
        public static async void Logout(Action callback)
        {
            await Dispatcher.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var state = await FacebookGateway.Instance.LogoutAsync();
                if (callback != null)
                {
                    Dispatcher.AppDispatcher(callback);
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
            await Dispatcher.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (callback != null)
                { 
                    var state = await FacebookGateway.Instance.LoginAsync();
                    if (state == NavigationState.Done && callback != null)
                    {
                        Dispatcher.AppDispatcher(() => callback(FacebookGateway.Instance.AccessToken));
                    }
                }
            });
#endif
        }

    }

}