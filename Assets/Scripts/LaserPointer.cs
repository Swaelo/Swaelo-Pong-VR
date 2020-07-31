// ================================================================================================================================
// File:        LaserPointer.cs
// Description:	Shoots lasers from the controllers to aim where the player wants to teleport
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using Valve.VR;

public class LaserPointer : MonoBehaviour
{
    public SteamVR_Input_Sources HandType;  //Which hand this laser is being shot from
    public SteamVR_Behaviour_Pose ControllerPose;   //Which controller pose to read for aiming
    public SteamVR_Action_Boolean TeleportAction;   //Input action which triggers teleporting

    public GameObject LaserPrefab;  //Prefab mesh used to render the laser pointer effect
    private GameObject LaserObject; //Instance of the laser pointer object
    private Transform LaserTransform;   //Transform of the laser object
    private Vector3 HitPoint;   //Stored collision point of raycast fired from controller

    public Transform CameraRigTransform;    //Transform component of the players SteamVR CameraRig object
    public GameObject TeleportReticlePrefab;    //Prefab mesh used to render the teleport targetting reticle
    private GameObject TeleportReticle; //Instance of the teleport targetting reticle object
    private Transform ReticleTransform; //Transform of the teleport targetting reticle
    public Transform HeadTransform; //Transform component of the players vr head object
    public Vector3 TeleportReticleOffset;   //Teleport reticles offset from the floor to stop z-fighting
    public LayerMask TeleportMask;  //Collision layermask used when firing raycasts for aiming teleports
    private bool ShouldTeleport;    //True when a valid teleport location has been found

    private void Start()
    {
        //Instantiate the laser point object and store reference to its transform component
        LaserObject = Instantiate(LaserPrefab);
        LaserTransform = LaserObject.transform;
        //Instantiate the teleport reticle object and store reference to its transform component
        TeleportReticle = Instantiate(TeleportReticlePrefab);
        ReticleTransform = TeleportReticle.transform;
    }

    private void Update()
    {
        //Show the laser point / targetting reticle wherever the players controller is aiming
        FireLaserPointer();

        //Teleport to the targetting reticle when the button is pressed
        if (TeleportAction.GetStateUp(HandType) && ShouldTeleport)
            Teleport();
    }

    //Fires a raycast from the controller, stores collision point info and draws a laser to that location
    private void FireLaserPointer()
    {
        //Fire a raycast from the controller
        RaycastHit RayHit;
        if (Physics.Raycast(ControllerPose.transform.position, transform.forward, out RayHit, 100))
        {
            //Store the hitpoint, show the laser point to that location and enable teleportation
            HitPoint = RayHit.point;
            ShowLaserPointer(RayHit);
            TeleportReticle.SetActive(true);
            ReticleTransform.position = HitPoint + TeleportReticleOffset;
            ShouldTeleport = true;
        }
        else
        {
            //Hide the laser and reticle if no hitpoint was found
            LaserObject.SetActive(false);
            TeleportReticle.SetActive(false);
        }
    }

    //Makes the laser pointer effect start to be rendered
    private void ShowLaserPointer(RaycastHit RayHit)
    {
        LaserObject.SetActive(true);
        LaserTransform.position = Vector3.Lerp(ControllerPose.transform.position, HitPoint, 0.5f);
        LaserTransform.LookAt(HitPoint);
        LaserTransform.localScale = new Vector3(LaserTransform.localScale.x,
            LaserTransform.localScale.y, RayHit.distance);
    }

    //Teleports the player to the location of the targetting reticle
    private void Teleport()
    {
        ShouldTeleport = false;
        TeleportReticle.SetActive(false);
        Vector3 Difference = CameraRigTransform.position - HeadTransform.position;
        Difference.y = 0f;
        CameraRigTransform.position = HitPoint + Difference;
    }
}
