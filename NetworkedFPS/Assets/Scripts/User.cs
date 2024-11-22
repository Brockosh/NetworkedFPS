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
    public GameObject FirstPersonPlayerPrefab;

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
        DontDestroyOnLoad(gameObject);
    }


    public void UpdateControlledObject(string newSceneName)
    {
        if (newSceneName.StartsWith("Scene_Map"))
        {
            CmdSpawn();
            NetworkRoomPlayerLobby room = GetComponentInChildren<NetworkRoomPlayerLobby>();
            Destroy(room);
        }
    }

    [Command]
    public void CmdSpawn()
    {
        GameObject firstPersonPlayer = Instantiate(FirstPersonPlayerPrefab);
        NetworkServer.Spawn(firstPersonPlayer, gameObject);
    }
}
