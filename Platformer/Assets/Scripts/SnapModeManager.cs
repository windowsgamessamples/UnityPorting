using UnityEngine;
using System.Collections;

public class SnapModeManager : MonoBehaviour
{
#if UNITY_METRO
    private bool _snapped = false;

    #region Instance

    static SnapModeManager _instance;
    public static SnapModeManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(SnapModeManager)) as SnapModeManager;
            return _instance;
        }
        set { _instance = value; }
    }
#endregion

	void Update () {
        if (Input.GetKeyDown(KeyCode.O))
            Show();

        if(Input.GetKeyDown(KeyCode.P))
            Hide();
	}

    public void Show()
    {
        GameManager.Instance.Pause();
        _snapped = true;
    }

    public void Hide()
    {
        GameManager.Instance.Resume();
        _snapped = false;
    }

    void OnGUI()
    {
        if (!_snapped)
            return;

        if(GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100), "Resume Game"))
        {
            Hide();
        }
    }
#endif
}
