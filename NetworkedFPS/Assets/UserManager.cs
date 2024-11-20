using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : NetworkBehaviour
{
    public User localUser;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OnSceneChanged(string sceneName)
    {
        localUser.UpdateControlledObject(sceneName);
    }


    [TargetRpc]
    public void SetLocalUser(NetworkConnectionToClient conn, GameObject user)
    {
        if (!isClient) return;
        localUser = user.GetComponent<User>();
    }

}
