using UnityEngine;

/// <summary>
/// Handles Facebook integration
/// </summary>
public class FacebookManager : MonoBehaviour
{
    private bool _loggedIn = false;
    private const string FacebookAuthTokenKey = "FacebookAuthToken";

    GUIContent[] comboBoxList;
    private ComboBox comboBoxControl;// = new ComboBox();
    private GUIStyle listStyle = new GUIStyle();

    void Start()
    {
        comboBoxList = new GUIContent[5];
        comboBoxList[0] = new GUIContent("Friend 1");
        comboBoxList[1] = new GUIContent("Friend 2");
        comboBoxList[2] = new GUIContent("Friend 3");
        comboBoxList[3] = new GUIContent("Friend 4");
        comboBoxList[4] = new GUIContent("Friend 5");

        listStyle.normal.textColor = Color.white;
        listStyle.onHover.background =
        listStyle.hover.background = new Texture2D(2, 2);
        listStyle.padding.left =
        listStyle.padding.right =
        listStyle.padding.top =
        listStyle.padding.bottom = 4;

        comboBoxControl = new ComboBox(new Rect(160, 20, 100, 20), comboBoxList[0], comboBoxList, "button", "box", listStyle);
    }

    void OnGUI()
    {
        comboBoxControl.Show();
        
        GUI.Label(new Rect(20, 20, 150, 20), "Select friend to invite : " + comboBoxControl.ButtonContent);
        if (GUI.Button(new Rect(280, 20, 100, 20), "Send invite"))
        {
            ShowFriends();
        }

        if (_loggedIn)
        {
            if (GUI.Button(new Rect(Screen.width - 150, 50, 130, 20), "Facebook logout"))
            {
                Logout();
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
