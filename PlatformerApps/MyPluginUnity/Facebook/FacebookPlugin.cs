using MyPlugin;
using System;
#if NETFX_CORE
using Windows.UI.Core;
#endif

namespace MyPlugin.Facebook
{

    public static class FacebookPlugin
    {

        public static void InviteFriend(string friendName)
        {
#if NETFX_CORE
            Dispatcher.InvokeOnUIThread(async () =>
            {
                await FacebookGateway.Instance.InviteFriendsAsync(friendName);
            });
#endif
        }

        public static void Logout(Action callback)
        {
#if NETFX_CORE
            Dispatcher.InvokeOnUIThread(async () =>
            {
                var state = await FacebookGateway.Instance.LogoutAsync();
                if (callback != null)
                {
                    Dispatcher.InvokeOnAppThread(callback);
                }
            });
#endif
        }

        public static void Login(Action<string> callback)
        {
#if NETFX_CORE
            Dispatcher.InvokeOnUIThread(async () =>
            {
                if (callback != null)
                { 
                    var state = await FacebookGateway.Instance.LoginAsync();
                    if (state == NavigationState.Done && callback != null)
                    {
                        Dispatcher.InvokeOnAppThread(() => callback(FacebookGateway.Instance.AccessToken));
                    }
                }
            });
#endif
        }

    }

}