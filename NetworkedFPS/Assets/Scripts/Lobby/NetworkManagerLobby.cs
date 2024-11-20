using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene][SerializeField] private string menuScene = string.Empty;


    // Each client and the server has its own roomPlayerPrefab, this will automatically become active when the start host
    // is called
    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("User")]
    [SerializeField] private User userPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;

    [Header("UserManager")]
    [SerializeField] private UserManager userManager = null;


    // Called when a client connects 
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action<string> OnSceneChange;
    public static event Action OnUserManagerReady;

    public List<User> Users { get; } = new List<User>();
    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();


    // This would be run when Start Host was ran
    // Prepares all prefabs that might need to be spawned
    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
        UserManager userManagerRef = Instantiate(userManager);
        NetworkServer.Spawn(userManagerRef.gameObject);
        userManager = userManagerRef;
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            // I think allows us to set from the editor what prefabs the client will be able to spawn
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        // Access the current client's connection if needed
        var connection = NetworkClient.connection;

        if (connection != null)
        {
            Debug.Log("Client connected with connection ID: " + connection.connectionId);
        }

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        // Access the current client's connection if needed
        var connection = NetworkClient.connection;

        if (connection != null)
        {
            Debug.Log("Client disconnected with connection ID: " + connection.connectionId);
        }

        OnClientDisconnected?.Invoke();
    }


    // Called when a new client connects
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

 
        if (SceneManager.GetActiveScene().name != "menuScene")
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<User>();

            Users.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);

    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var user in Users)
        {
            NetworkRoomPlayerLobby userRoomPlayer = user.GetComponent<NetworkRoomPlayerLobby>();
            userRoomPlayer.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; };
        }

        return true;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Makes sure the client is in the right scene
        if (SceneManager.GetActiveScene().name == "menuScene")
        {
            bool IsHost = Users.Count == 0;

            User userInstance = Instantiate(userPrefab);

            userPrefab.IsHost= IsHost;

            NetworkServer.AddPlayerForConnection(conn, userInstance.gameObject);

            if (conn.identity != null)
            {
                userManager.SetLocalUser(conn, userInstance.gameObject);
                
            }


            // Pretty sure this will need to be replaced with User instead

            //bool isLeader = RoomPlayers.Count == 0;

            //NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

            //roomPlayerInstance.IsLeader = isLeader;

            //NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnStopServer()
    {
        Users.Clear();
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == "menuScene")
        {
            if (!IsReadyToStart()) { return; }

            ServerChangeScene("Scene_Map_01");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().name == "menuScene" && newSceneName.StartsWith("Scene_Map"))
        {
            for (int i = Users.Count - 1; i >= 0; i--)
            {




                //var conn = RoomPlayers[i].connectionToClient;
                //var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                //gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                //NetworkServer.Destroy(conn.identity.gameObject);

                //NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("Scene_Map"))
        {
           // GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            //NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        //OnSceneChange?.Invoke("Scene_Map_01");
        userManager.OnSceneChanged("Scene_Map_01");
        
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

}
