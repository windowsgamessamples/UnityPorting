using UnityEngine;
#if UNITY_WINRT
using System;
using System.Collections;
using System.Threading;
using System.IO;

/// <summary>
/// Windows specific and interop between Unity and Windows Store or Windows Phone 8
/// </summary>
public static class WindowsGateway
{

    static WindowsGateway()
    {

        // create blank implementations to avoid errors within editor
        UnityLoaded = delegate {};
    }

    internal static void Initialize()
    {

#if UNITY_METRO

        // unity now supports handling size changed in 4.3
        UnityEngine.WSA.Application.windowSizeChanged += WindowSizeChanged;

        // unity supports tracking window activation/deactivation in 4.3 (not app activation!)
        UnityEngine.WSA.Application.windowActivated += Application_windowActivated;

#endif

    }

    /// <summary>
    /// Called from Unity when the app is responsive and ready for play, picked up by the app
    /// </summary>
    public static Action UnityLoaded;

#if UNITY_METRO 

    /// <summary>
    /// Handler for window activation/deactivation (not app activation!)
    /// </summary>
    private static void Application_windowActivated(UnityEngine.WSA.WindowActivationState state)
    {
        if (state == UnityEngine.WSA.WindowActivationState.CodeActivated && MyPlugin.WindowsPlugin.Instance.OrientationChanged == null)
        {
            // our plugin allows us to handle orientation change events
            MyPlugin.WindowsPlugin.Instance.OrientationChanged = OrientationChanged;
        }
    }

    /// <summary>
    /// Allows handling of orientation changed event
    /// </summary>
    private static void OrientationChanged(object sender, EventArgs eventArgs)
    { 
        
    }

    /// <summary>
    /// Deal with windows resizing
    /// </summary>
    public static void WindowSizeChanged(int width, int height) 
    {
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