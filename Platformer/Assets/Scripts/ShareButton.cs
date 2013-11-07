using UnityEngine;

public class ShareButton : MonoBehaviour 
{
    void OnGUI()
    {
#if UNITY_WINRT
        if (GUI.Button(new Rect(Screen.width - 120, 20, 100, 20), "Share"))
            WindowsGateway.ShowShareUI();
#endif
    }
}
