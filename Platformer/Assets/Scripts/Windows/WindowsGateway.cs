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

        // simple call to some WACK tests
        RunWACKSamples();

    }

    /// <summary>
    /// rudimentary hooks using our WACK override classes (unit tests would be best)
    /// </summary>
    private static void RunWACKSamples()
    {
        // some games use thread.sleep, not in UNITY_METRO
        Thread.Sleep(1); 

        // these collections aren't in UNITY_METRO or UNITY_WP8
        var hash = new Hashtable();
        hash.Add("1", "first");
        var alist = new ArrayList();
        alist.Add("test");

        if (File.Exists("test"))
        {

        }
        if (Directory.Exists("test2"))
        {
        }

    }

    /// <summary>
    /// Called from Unity when the app is invoking the share charm, demonstrates plugin approach
    /// </summary>
    public static void ShowShareUI()
    {
        MyPlugin.WindowsPlugin.ShowShareUI();
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
