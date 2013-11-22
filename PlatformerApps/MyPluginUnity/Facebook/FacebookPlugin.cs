using MyPlugin;
using System;
using System.Collections.Generic;

#if NETFX_CORE || WINDOWS_PHONE
using Windows.UI.Core;
#endif


namespace MyPlugin.Facebook
{

    public static class FacebookPlugin
    {

        public static void GetFriends(Action<List<FacebookUser>> callback)
        {
#if NETFX_CORE || WINDOWS_PHONE
            Dispatcher.InvokeOnUIThread(async () =>
            {
                var friends = await FacebookGateway.Instance.GetFriends();
                if (callback != null)
                {
                    Dispatcher.InvokeOnAppThread(() => callback(friends));
                }
            });
#endif
        }

        public static void InviteFriend(string friendName)
        {
#if NETFX_CORE || WINDOWS_PHONE
            Dispatcher.InvokeOnUIThread(async () =>
            {
                await FacebookGateway.Instance.InviteFriendsAsync(friendName);
            });
#endif
        }

        public static void Logout(Action callback)
        {
#if NETFX_CORE || WINDOWS_PHONE
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
#if NETFX_CORE || WINDOWS_PHONE
            Dispatcher.InvokeOnUIThread(async () =>
            {
                if (callback != null)
                { 
                    var state = await FacebookGateway.Instance.LoginAsync();
                    if (state == NavigationState.Done)
                    {
                        Dispatcher.InvokeOnAppThread(() => callback(FacebookGateway.Instance.AccessToken));
                    }
                }
            });
#endif
        }

    }

}