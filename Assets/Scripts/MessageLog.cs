// ================================================================================================================================
// File:        MessageLog.cs
// Description:	Display debug messages in a window to the user during testing
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class MessageLog : MonoBehaviour
{
    //Singleton
    public static MessageLog Instance = null;
    private void Awake() { Instance = this; }

    //Message window and array of previous messages
    public Text WindowContents;
    public string[] LogMessages = new string[15];

    public static void Print(string Message)
    {
        //Move all the previous messages back 1 line
        for (int i = 14; i > 0; i--)
            MessageLog.Instance.LogMessages[i] = MessageLog.Instance.LogMessages[i - 1];
        //Place the new message at the front
        MessageLog.Instance.LogMessages[0] = Message;

        //Update the window display
        string WindowMessage = "";
        for (int i = 14; i > -1; i--)
            WindowMessage += MessageLog.Instance.LogMessages[i] + "\n";
        MessageLog.Instance.WindowContents.text = WindowMessage;
    }
}
