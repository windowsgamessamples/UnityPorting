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

#if UNITY_METRO

        // unity now supports handling size changed in 4.3
        UnityEngine.WSA.Application.windowSizeChanged += WindowSizeChanged;

#endif

        // create blank implementations to avoid errors within editor
        UnityLoaded = delegate {};

    }

    /// <summary>
    /// Called from Unity when the app is responsive and ready for play
    /// </summary>
    public static Action UnityLoaded;

    /// <summary>
    /// Used when the game window becomes available again, and based on the settings determined whether to play sound or not
    /// </summary>
    public static void InitialiseSound()
    {
        GameManager.Instance.InitialiseSound();
    }

    /// <summary>
    /// Toggle sound on/off based on parameter
    /// </summary>
    /// <param name="on"></param>
    public static void EnableSound(bool on)
    {
        GameManager.Instance.EnableSound(on);
    }

    /// <summary>
    /// Called from the Win 8 solution to determine if sound is enabled or not
    /// </summary>
    /// <returns></returns>
    public static bool IsSoundEnabled()
    {
        return GameManager.Instance.IsSoundEnabled();
    }

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