using UnityEngine;
#if UNITY_WINRT
using System;
using System.Collections;
using System.Threading;
using System.IO;

/// <summary>
/// Interop between Unity and Windows Store App
/// </summary>
public static class WindowsGateway
{

    static WindowsGateway()
    {
#if UNITY_METRO
        { 
            UnityEngine.WSA.Application.windowSizeChanged += WindowSizeChanged;
        }
#endif

        // create blank implementations to avvoid errors within editor
        UnityLoaded = delegate {};
    }

    /// <summary>
    /// Called from Unity when the app is invoking the share charm, demonstrates plugin approach
    /// </summary>
    public static void ShowShareUI()
    {
#if UNITY_METRO && !UNITY_EDITOR
        MyPlugin.WindowsPlugin.ShowShareUI();
#endif
    }

    /// <summary>
    /// This calls the XAML overlay to login to facebook
    /// It accepts a gameobject and method as a callback method when login is successful
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="callback"></param>
    public static void FacebookLogin(string gameObject, string callback)
    {     
#if UNITY_METRO && !UNITY_EDITOR
        MyPlugin.Facebook.FacebookPlugin.Login(result =>
        {
            var obj = GameObject.Find(gameObject);
            if (obj != null) obj.SendMessage(callback, result);
        });
#endif
    }

    /// <summary>
    /// Log the user out of facebook
    /// </summary>
    public static void FacebookLogout()
    {
#if UNITY_METRO && !UNITY_EDITOR
        MyPlugin.Facebook.FacebookPlugin.Logout(null);
#endif
    }


    /// <summary>
    /// Called from Unity when the app is responsive and ready for play
    /// </summary>
    public static Action UnityLoaded;

#if UNITY_METRO

    /// <summary>
    /// Deal with windows resizing
    /// </summary>
    public static void WindowSizeChanged(int width, int height) 
    {
	    // TODO deal with window resizing. e.g. if <= 500 implement pause screen
        if (width <= 500)
        {
            SnapModeManager.Instance.Show();
        }
        else
        {
            SnapModeManager.Instance.Hide();
        }
	} 
  
#endif

}

#endif
