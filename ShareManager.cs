using UnityEngine;

/// <summary>
/// Handles Share Integration
/// </summary>
public class ShareManager : MonoBehaviour 
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 120, 20, 100, 20), "Share"))
        {
#if UNITY_WINRT
            MyPlugin.WindowsPlugin.ShowShareUI();
#endif
        }
    }
}
