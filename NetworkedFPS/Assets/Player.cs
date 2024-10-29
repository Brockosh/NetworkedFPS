using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public Transform cameraAttachPoint;
    public Transform gunAttachPoint;
    public GunSystem M4; 

    void Start()
    {
        Camera camera = FindObjectOfType<Camera>();

        if (isLocalPlayer)
        {
            Transform cameraTransform = camera.transform;
            cameraTransform.parent = cameraAttachPoint.transform;  
            cameraTransform.position = cameraAttachPoint.transform.position;  
            cameraTransform.rotation = cameraAttachPoint.transform.rotation;

            camera.GetComponent<MouseLook>().playerBody = transform;
            camera.GetComponent<MouseLook>().attachedToPlayerTransform = true;
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
