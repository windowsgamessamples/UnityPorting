using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public bool Paused { get; set; }
    private float _originalTimeScale = 1;
    private Score _score;
    private bool _showConfirmQuit = false;
    private bool _showResume = false;
    private string _version;

    [SerializeField] private GameObject _facebookObject;
    [SerializeField] private GameObject _shareObject ;
    [SerializeField] private GameObject _musicObject;

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

    void Start ()
    {
        _version = MyPlugin.WindowsPlugin.Instance.GetAppVersion();
        _score = GameObject.Find("Score").GetComponent<Score>();
        
        if (_facebookObject != null)
            _facebookObject.AddComponent<FacebookManager>();

        if (_shareObject != null)
            _shareObject.AddComponent<ShareManager>();

        InitialiseSound();

#if UNITY_WINRT && !UNITY_EDITOR  
        WindowsGateway.Initialize();
        WindowsGateway.UnityLoaded();
#endif

    }

    public void InitialiseSound()
    {
        // Enable/disable the sound based on how it was previously set in game settings
        string soundEnabled = PlayerPrefs.GetString("soundenabled");

        if (soundEnabled == "")
            EnableSound(true);
        else
            EnableSound(soundEnabled == "True");
    }

    public void EnableSound(bool on)
    {
        if (on)
        {
            _musicObject.GetComponent<AudioSource>().Play();
            AudioListener.pause = false;
        }
        else
        {
            _musicObject.GetComponent<AudioSource>().Stop();
            AudioListener.pause = true;
            
        }

        PlayerPrefs.SetString("soundenabled", on.ToString());
        PlayerPrefs.Save();
    }

    public bool IsSoundEnabled()
    {
        string soundEnabled = PlayerPrefs.GetString("soundenabled");

        if (soundEnabled == "")
        {
            PlayerPrefs.SetString("soundenabled", "True");
            PlayerPrefs.Save();
            return true;
        }
        else
            return Convert.ToBoolean(soundEnabled);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
            _showConfirmQuit = true;
        }
    }

    void OnGUI()
    {
        if (_showConfirmQuit)
        {
            if (GUI.Button(new Rect(Screen.width/2 - 100, Screen.height/2 - 50, 100, 100), "Confirm Quit"))
                Application.Quit();
            if (GUI.Button(new Rect(Screen.width/2 + 50, Screen.height/2 - 50, 100, 100), "Cancel"))
            {
                _showConfirmQuit = false;
                Resume();
            }
        }

        if (_showResume)
        {
            if (GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 100, 100), "Resume Game"))
            {
                _showResume = false;
                Resume();
            }
        }

        // Show version number of app
        GUI.Label(new Rect(Screen.width - 120, 20, 100, 20), "Version : " + _version);
    }

    public void ShowResume()
    {
        _showResume = true;
    }
	
    // Pause the game
    public void Pause()
    {
        // Stop all audio from playing
        AudioListener.pause = true;

        // Set the game's timescale to 0
        _originalTimeScale = Time.timeScale;
        Time.timeScale = 0; 

        Paused = true;
    }

    // Unpause the game
    public void Resume()
    {
        // Resume all audio playing
        AudioListener.pause = false;

        // Return game's timescale to original value
        Time.timeScale = _originalTimeScale;

        Paused = false;
    }

    public int GetScore()
    {
        return _score.GetScore();
    }
}
