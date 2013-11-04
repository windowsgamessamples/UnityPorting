#if UNITY_METRO

using UnityEngine;
using System.Collections;

/// <summary>
/// Receives calls directly from Windows Store App
/// </summary>
public static class WindowsGateway  {

	// called when window is resized
	public static void WindowSizeChanged (double height, double width) {
	    // TODO deal with window resizing. e.g. if <= 500 implement pause screen
	}
	

}

#endif
