using UnityEngine;

/// <summary>
/// Handles Share Integration
/// </summary>
public class ShareManager : MonoBehaviour
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 200, 40), "Share"))
        {
#if UNITY_WINRT
            MyPlugin.WindowsPlugin.Instance.ShowShareUI();
#endif
        }
    }
}
