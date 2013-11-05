using UnityEngine;
using System.Collections;

public class SnapModeManager : MonoBehaviour {

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

	void Start () {
	
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.O))
            Show();

        if(Input.GetKeyDown(KeyCode.P))
            Hide();
	}

    public void Show()
    {
        GameManager.Instance.Pause();
    }

    public void Hide()
    {
        GameManager.Instance.Resume();
    }

    void OnGUI()
    {
        if (!GameManager.Instance.Paused)
            return;

        if(GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100), "Resume Game"))
        {
            Hide();
        }
    }
}
