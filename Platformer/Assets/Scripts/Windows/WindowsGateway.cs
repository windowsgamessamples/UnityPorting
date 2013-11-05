#if UNITY_METRO

using System;
using System.Collections;

/// <summary>
/// Interop between Unity and Windows Store App
/// </summary>
public class WindowsGateway
{
    /// <summary>
    /// Called from Unity when the app is responsive and ready for play
    /// </summary>
    public static Action UnityLoaded;

    /// <summary>
    /// Called from Unity when the app is invokeing the share charm 
    /// </summary>
    public static Action ShowShareUI;

	/// <summary>
	/// Called from Windows Store app when the app's window is resized
	/// </summary>
	public static void WindowSizeChanged (double height, double width) 
    {
	    // TODO deal with window resizing. e.g. if <= 500 implement pause screen
	}

    public static void DoUnityLoaded()
    {
#if UNITY_METRO && !UNITY_EDITOR
        if (UnityLoaded != null)
        {
            UnityLoaded();
        }
#endif
    }
}

#endif
