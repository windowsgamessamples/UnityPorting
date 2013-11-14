using UnityEngine;

/// <summary>
/// Handles Facebook integration
/// </summary>
public class FacebookManager : MonoBehaviour
{
    private bool _loggedIn = false;
    private const string FacebookAuthTokenKey = "FacebookAuthToken";

    void OnGUI()
    {

        if (_loggedIn)
        {
            if (GUI.Button(new Rect(Screen.width - 150, 50, 130, 20), "Facebook logout"))
            {
                Logout();
            }

            if (GUI.Button(new Rect(Screen.width - 150, 80, 130, 20), "Invite friend"))
            {
                ShowFriends();
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width - 150, 50, 130, 20), "Facebook login"))
            {
                Login();
            }
        }
    }

    private void ShowFriends()
    {
#if UNITY_WINRT
        // TODO display a list of friends, and then when one is pressed call InviteFriend("friendid");
        InviteFriend("TestUser");
#endif
    }

    private void Login()
    {
#if UNITY_WINRT
        MyPlugin.Facebook.FacebookPlugin.Login(result =>
        {
            if (!string.IsNullOrEmpty(result))
            {
                PlayerPrefs.SetString(FacebookAuthTokenKey, result);
                PlayerPrefs.Save();
            }
        });
#endif
    }

    private void Logout()
    {
#if UNITY_WINRT
        MyPlugin.Facebook.FacebookPlugin.Logout(() =>
        {
            if (PlayerPrefs.HasKey(FacebookAuthTokenKey))
            {
                PlayerPrefs.DeleteKey(FacebookAuthTokenKey);
                PlayerPrefs.Save();
            }
            _loggedIn = false;
        });
#endif
    }

    private void InviteFriend(string friend)
    {
#if UNITY_WINRT
        MyPlugin.Facebook.FacebookPlugin.InviteFriend(friend);
#endif
    }

}
