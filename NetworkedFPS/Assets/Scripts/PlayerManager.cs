using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager instance;
    public event Action<GameObject> OnPlayerDied;
    public int respawnDelay = 3;

    public ObjectSpawner spawner;

    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CallOnPlayerDied(GameObject player)
    {
        OnPlayerDied?.Invoke(player);
    }

    public void PrepareRespawn(NetworkConnectionToClient conn, uint playerId)
    {
        Vector3 spawnPosition = spawner.FindSpawnPosition();
        RespawnPlayer(conn, playerId, spawnPosition);
    }

    [TargetRpc]
    public void RespawnPlayer(NetworkConnectionToClient conn, uint playerId, Vector3 spawnPos)
    {
        Debug.Log("RUNNING RESPAWN LOGIC HERE");
        
        //Player player = NetworkClient.spawned[playerId].GetComponent<Player>();
        //Vector3 moveVec = new Vector3(5, 0, 5);
        //Player player = NetworkClient.localPlayer.GetComponent<Player>();
        //player.transform.position = moveVec;
        Debug.LogWarning(spawnPos);
        Debug.LogWarning($"PLAYER POSITION of connection id {playerId} MOVED");
        // player.GetComponent<PlayerHealth>().ResetHealth();

        StartCoroutine(DelayMove(spawnPos));
    }

    IEnumerator DelayMove(Vector3 spawnPos)
    {
        yield return new WaitForEndOfFrame();
        Player player = NetworkClient.localPlayer.GetComponent<Player>();
        player.transform.position = spawnPos;

        Debug.Log("Move");
    }
}
