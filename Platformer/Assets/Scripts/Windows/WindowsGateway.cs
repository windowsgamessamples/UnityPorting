#if UNITY_METRO

using UnityEngine;
using System.Collections;

/// <summary>
/// Receives calls directly from Windows Store App
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

    public delegate void UnityLoadedHandler();
    public static event UnityLoadedHandler UnityLoaded;

	// called when window is resized
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

        // Code here to let the XAML app know that the first Unity scene has finished loading...
        UnityLoaded();

        // Now that our singleton WindowsGatewayObject exists, load the next game scene
        Application.LoadLevel(1);
    }
}

#endif
