// ================================================================================================================================
// File:        ActionsTest.cs
// Description:	Testing input actions for the VR controllers
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using Valve.VR;

public class ActionsTest : MonoBehaviour
{
    public SteamVR_Input_Sources HandType;  //Type of hand to read actions for
    public SteamVR_Action_Boolean TeleportAction;   //Teleport action to read
    public SteamVR_Action_Boolean GrabAction;   //Grab action to read

    //Functions return true when their specified input actions have been activated
    public bool IsTeleportDown() { return TeleportAction.GetStateDown(HandType); }
    public bool IsGrabDown() { return GrabAction.GetStateDown(HandType); }

    //Print messages to the log whenever the actions are activated
    private void Update()
    {
        if (IsTeleportDown())
            MessageLog.Print("Teleport Action Activated");
        if (IsGrabDown())
            MessageLog.Print("Grab Action Activated");
    }
}
