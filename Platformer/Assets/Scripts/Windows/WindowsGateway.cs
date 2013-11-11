#if UNITY_WINRT

using System;
using System.Collections;
using MyPlugin;

/// <summary>
/// Interop between Unity and Windows Store App
/// </summary>
public static class WindowsGateway
{

    static WindowsGateway()
    {
        UnityEngine.WSA.Application.windowSizeChanged += WindowSizeChanged;

        // create blank implementations to avvoid errors within editor
        UnityLoaded = delegate {};
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

	/// <summary>
	/// Called from Windows Store app when the app's window is resized
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

}

#endif
