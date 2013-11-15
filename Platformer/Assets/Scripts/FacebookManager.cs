using System.Collections.Generic;
using System.Linq;
using MyPlugin.Facebook;
using UnityEngine;

/// <summary>
/// Handles Facebook integration
/// </summary>
public class FacebookManager : MonoBehaviour
{
    private bool _loggedIn = false;
    private bool _showInviteFriendsDialog = false;
    private const string FacebookAuthTokenKey = "FacebookAuthToken";

    private GUIContent[] _friendsListGUIContent;
    private List<FacebookUser> _friendsList;
    private ComboBox _comboBoxControl;
    private GUIStyle _listStyle = new GUIStyle();

    void Start()
    {
        // If a facebook authenticaton token has been previously saved, try to login to Facebook automatically
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(FacebookAuthTokenKey)))
            Login();
    }

    public Rect inviteFriendsDialog = new Rect(500, 500, 500, 300);

    void RenderInviteFriendsDialog(int windowID)
    {
#if UNITY_WINRT
        if (_comboBoxControl == null)
            return;

        _comboBoxControl.Show();

        GUI.Label(new Rect(20, 50, 150, 20), "Select friend to invite : " + _comboBoxControl.SelectedText);
    
        if (GUI.Button(new Rect(20, 350, 100, 20), "Send invite"))
            InviteFriend(_comboBoxControl.SelectedText);

        if (GUI.Button(new Rect(280, 350, 100, 20), "Cancel"))
            _showInviteFriendsDialog = false;

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
#endif
    }

    void OnGUI()
    {
#if UNITY_METRO
        if(_showInviteFriendsDialog)
            inviteFriendsDialog = GUI.Window(0, new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 400), RenderInviteFriendsDialog, "Invite friends");

        if (_loggedIn)
        {
            if (GUI.Button(new Rect(Screen.width - 150, 50, 130, 20), "Facebook logout"))
                Logout();

            if (GUI.Button(new Rect(Screen.width - 150, 80, 130, 20), "Invite friends"))
            {
                _showInviteFriendsDialog = true;
                GetFriends();
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width - 150, 50, 130, 20), "Facebook login"))
                Login();
        }
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

                _loggedIn = true;
            }
            else
            {
                _loggedIn = false;
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
            _showInviteFriendsDialog = false;
        });
#endif
    }

    private void GetFriends()
    {
#if UNITY_WINRT
        MyPlugin.Facebook.FacebookPlugin.GetFriends(InitialiseFriendsListComboBox);
#endif
    }

    private void InitialiseFriendsListComboBox(List<FacebookUser> friends)
    {
#if UNITY_WINRT
        friends = friends.OrderBy(f => f.Name).Take(10).ToList();
        _friendsList = friends;
        _friendsListGUIContent = new GUIContent[friends.Count];

        for (int i = 0; i < friends.Count; i++)
            _friendsListGUIContent[i] = new GUIContent(friends[i].Name);

        _listStyle.normal.textColor = Color.white;
        _listStyle.onHover.background =
        _listStyle.hover.background = new Texture2D(2, 2);
        _listStyle.padding.left =
        _listStyle.padding.right =
        _listStyle.padding.top =
        _listStyle.padding.bottom = 4;

        _comboBoxControl = new ComboBox(new Rect(160, 50, 200, 20), _friendsListGUIContent[0], _friendsListGUIContent, "button", "box", _listStyle);
#endif
    }

    /// <summary>
    /// Pass in the selected friend name that was in the combobox, then retrieve the id from the _friendsList
    /// </summary>
    /// <param name="friendName"></param>
    private void InviteFriend(string friendName)
    {
#if UNITY_WINRT
        var selectedFriend = _friendsList.FirstOrDefault(f => f.Name == friendName);

        if (selectedFriend != null)
        {
            MyPlugin.Facebook.FacebookPlugin.InviteFriend(selectedFriend.Id);
            _showInviteFriendsDialog = false;
        }
#endif

    }
}
