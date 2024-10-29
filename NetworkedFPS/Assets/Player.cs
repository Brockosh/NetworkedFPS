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
            GunSystem gun = Instantiate(M4);
            gun.transform.parent = camera.transform;
            gun.transform.localPosition = gunAttachPoint.transform.localPosition;
            gun.transform.localRotation = gunAttachPoint.transform.localRotation;
            gun.AssignOwner(netIdentity.netId);
            NetworkServer.Spawn(gun.gameObject, connectionToClient);
        }
    }
}
