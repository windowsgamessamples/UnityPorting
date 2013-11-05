#if UNITY_METRO

using System;
using System.Collections;

/// <summary>
/// Interop between Unity and Windows Store App
/// </summary>
public static class WindowsGateway
{

    static WindowsGateway()
    {
        // create blank implementations to avvoid errors within editor
        UnityLoaded = delegate {};
        ShowShareUI = delegate {};
    }

    /// <summary>
    /// Called from Unity when the app is responsive and ready for play
    /// </summary>
    public static Action UnityLoaded;

    /// <summary>
    /// Called from Unity when the app is invoking the share charm 
    /// </summary>
    public static Action ShowShareUI;

	/// <summary>
	/// Called from Windows Store app when the app's window is resized
	/// </summary>
	public static void WindowSizeChanged (double height, double width) 
    {
	    // TODO deal with window resizing. e.g. if <= 500 implement pause screen
        if (width <= 500)
            SnapModeManager.Instance.Show();
        else
            SnapModeManager.Instance.Hide();
	}   

}

#endif
