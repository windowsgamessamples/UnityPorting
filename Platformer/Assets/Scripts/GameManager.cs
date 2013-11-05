using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public bool Paused { get; set; }
    private float _originalTimeScale;

    #region Instance
    static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            return _instance;
        }
        set { _instance = value;} 
    }
    #endregion

    void Start () {
#if UNITY_METRO && !UNITY_EDITOR  
        WindowsGateway.UnityLoaded();
#endif
	}
	
	void Update () {
	
	}

    // Pause the game
    public void Pause()
    {
        // Stop all audio from playing
        AudioListener.pause = true;

        _originalTimeScale = Time.timeScale;
        Time.timeScale = 0;

        Paused = true;
    }

    // Unpause the game
    public void Resume()
    {
        // Resume all audio playing
        AudioListener.pause = false;

        Time.timeScale = _originalTimeScale;

        Paused = false;
    }
}
