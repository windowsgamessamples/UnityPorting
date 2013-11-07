using UnityEngine;

public class ShareButton : MonoBehaviour 
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 120, 20, 100, 20), "Share"))
            WindowsGateway.ShowShareUI();
    }
}
