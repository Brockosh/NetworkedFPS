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
        if (isLocalPlayer)
        {
            Transform cameraTransform = FindObjectOfType<Camera>().transform;
            cameraTransform.parent = cameraAttachPoint.transform;  
            cameraTransform.position = cameraAttachPoint.transform.position;  
            cameraTransform.rotation = cameraAttachPoint.transform.rotation;

            cameraTransform.gameObject.GetComponent<MouseLook>().playerBody = transform;
            cameraTransform.gameObject.GetComponent<MouseLook>().attachedToPlayerTransform = true;
            cameraTransform.gameObject.GetComponent<MouseLook>().LockCursor();
        }

        if (NetworkServer.active)
        {
            GunSystem gun = Instantiate(M4);
            gun.transform.parent = gunAttachPoint.transform;
            gun.transform.localPosition = Vector3.zero;  
            gun.transform.localRotation = Quaternion.identity;
            gun.AssignOwner(netIdentity.netId);
            NetworkServer.Spawn(gun.gameObject, connectionToClient);
        }
    }
}
