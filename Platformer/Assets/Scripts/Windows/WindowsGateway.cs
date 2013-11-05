#if UNITY_METRO

using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Interop between Unity and Windows Store App
/// </summary>
public class WindowsGateway : MonoBehaviour
{
    static WindowsGateway _instance;
    public static WindowsGateway Instance
    {
        get        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(WindowsGateway)) as WindowsGateway;
            return _instance;
        }
        set { _instance = value; }
    }

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

    private void Awake()
    {
        _instance = this;

        // Don't destroy this object, so any public methods in this class can be referred to from any script in our game
        // Usage : WindowsGateway.Instance.YourPublicMethodName();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
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
