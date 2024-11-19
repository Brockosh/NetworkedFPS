using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public Transform cameraAttachPoint;
    public Transform gunAttachPoint;
    public GunSystem M4;

    public string playerName;

    void Start()
    {
        playerName = "Test";
       Camera camera = GetComponent<Camera>();
    }

    public override void OnStartAuthority()
    {
        Debug.Log($"Authority gained on object {gameObject.name}. IsLocalPlayer: {isLocalPlayer}");

        playerName = "Test";
        Camera camera = GetComponent<Camera>();


        //This is always false
        if (isLocalPlayer)
        {
            camera.GetComponent<MouseLook>().playerBody = transform;
            camera.GetComponent<MouseLook>().LockCursor();
        }

        if (NetworkServer.active)
        {
            //Create gun in server scene
            GunSystem gun = Instantiate(M4);
            //Spawn gun on server, meaning is shows up for the client
            gun.AssignOwner(netIdentity.netId);
            NetworkServer.Spawn(gun.gameObject, connectionToClient);
        }
    }

}
