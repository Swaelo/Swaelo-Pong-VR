// ================================================================================================================================
// File:        GrabObject.cs
// Description:	Allows the player to grab objects with their hands
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using Valve.VR;

public class GrabObject : MonoBehaviour
{
    public SteamVR_Input_Sources HandType;  //Which hand is going to be used for grabbing
    public SteamVR_Behaviour_Pose ControllerPose;   //Which controller will be used to control the hand
    public SteamVR_Action_Boolean GrabAction;   //Grab action that can be used to pickup objects

    private GameObject CollidingObject; //Tracks whatever gameobject the controller is currently colliding with so it can be grabbed
    private GameObject HeldObject;  //Object currently being held by the player

    //Takes a collider and uses its gameobject as the collidingobject for holding by the player
    private void SetCollidingObject(Collider Object)
    {
        //Ignore objects already being tracked as the colliding or held object
        if (CollidingObject == Object.gameObject || HeldObject == Object.gameObject)
            return;

        //Ignore objects without any rigidbody components
        if (!Object.GetComponent<Rigidbody>())
            return;

        //MessageLog.Print(Object.transform.name + " no colliding with " + gameObject.transform.name);
        CollidingObject = Object.gameObject;
    }

    //Set objects as the colliding object as the players hands come into contact with them
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    //Keep objects as the colliding object as the players hands continue touching them
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    //Stop tracking the colliding object if the player stops touching it
    public void OnTriggerExit(Collider other)
    {
        //Ignore if the player stops touching something that wasnt the colliding object
        if (!CollidingObject)
            return;

        //Clear the colliding object when the player stops touching it
        CollidingObject = null;
    }

    //Allows the player to grab the object in their hand
    private void Grab()
    {
        //Track this object is now being grabbed
        HeldObject = CollidingObject;
        CollidingObject = null;

        //Fix the object to the players hand
        var Joint = AddFixedJoint();
        Joint.connectedBody = HeldObject.GetComponent<Rigidbody>();
    }

    //Creates a fixed joint between the grabbed object and the players hand so they can hold onto it
    private FixedJoint AddFixedJoint()
    {
        FixedJoint NewJoint = gameObject.AddComponent<FixedJoint>();
        NewJoint.breakForce = 20000;
        NewJoint.breakTorque = 20000;
        return NewJoint;
    }

    //Allows the player to release the object in their hand and drop it
    private void Release()
    {
        if(GetComponent<FixedJoint>())
        {
            //Remove the joint being used to hold onto the object
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            //Transfer the velocity of the players hand to the object being dropped so they can throw the objects
            HeldObject.GetComponent<Rigidbody>().velocity = ControllerPose.GetVelocity();
            HeldObject.GetComponent<Rigidbody>().angularVelocity = ControllerPose.GetAngularVelocity();
        }

        //Stop tracking the item as being held
        HeldObject = null;
    }

    private void Update()
    {
        //Grab any object in the players hand when they perform the grab action
        if (GrabAction.GetLastStateDown(HandType))
        {
            if(CollidingObject)
                Grab();
        }

        //Release any object being held by the player while holding something and performing the grab action again
        if (GrabAction.GetLastStateUp(HandType))
        {
            if(HeldObject)
                Release();
        }
    }
}
