using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class User : NetworkBehaviour
{
    //[SyncVar]
    private string displayName = "Loading...";

    private bool isHost;
    public bool IsHost
    {
        get
        {
            return (isHost);
        }
        set
        {
            isHost = value;
        }
    }


    [SerializeField]
    private GameObject FirstPersonPlayerPrefab;

    private NetworkManagerLobby room;

    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }


    private void Start()
    {
        NetworkManagerLobby.OnSceneChange += UpdateControlledObject;
        DontDestroyOnLoad(this);
       
    }


    [Command]
    private void UpdateControlledObject(string newSceneName)
    {
        if (newSceneName.StartsWith("Scene_Map"))
        {
            Instantiate(FirstPersonPlayerPrefab);
            NetworkServer.Spawn(FirstPersonPlayerPrefab, connectionToServer);
        }
    }








}
