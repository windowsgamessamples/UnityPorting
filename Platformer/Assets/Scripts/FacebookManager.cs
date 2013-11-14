using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    private bool _loggedIn = false;

#if UNITY_WINRT
    void OnGUI()
    {
        if (_loggedIn)
        {
            if (GUI.Button(new Rect(Screen.width - 150, 50, 130, 20), "Facebook logout"))
            {
                WindowsGateway.FacebookLogout();
                _loggedIn = false;
            }

            if (GUI.Button(new Rect(Screen.width - 150, 80, 130, 20), "Invite friend"))
            {
                WindowsGateway.FacebookInviteFriend("damian.karzon");
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width - 150, 50, 130, 20), "Facebook login"))
                WindowsGateway.FacebookLogin(gameObject.name, "LoginSuccessful");
        }
    }

    /// <summary>
    /// This is our callback method that is called when the facebook login is a success 
    /// </summary>
    public void LoginSuccessful()
    {
        _loggedIn = true;
    }
#endif
}
