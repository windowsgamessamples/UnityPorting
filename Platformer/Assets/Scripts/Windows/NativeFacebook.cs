using System;

public static class NativeFacebook
{
    public static Action<Action<string>> DoLogin;
    public static Action<Action> DoLogout;
    public static Action<string> DoInviteFriend;

    public static void Login(Action<string> callback)
    {
        if (DoLogin != null)
            DoLogin(callback);
    }

    public static void Logout(Action callback)
    {
        if (DoLogout != null)
            DoLogout(callback);
    }

    public static void InviteFriend(string username)
    {
        if (DoInviteFriend != null)
            DoInviteFriend(username);
    }
}
