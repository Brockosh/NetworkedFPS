using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);
        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;


    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if (spawnPoint == null)
        {
            Debug.Log($"Missing spawn point for player {nextIndex}");
            return;
        }

        //Players spawn here, and are correctly position etc, but the problem I'm having is that these players are not in any way
        //associated with the roomPlayers from the menu or the logic that is in the ServerChangeScene method in the NetworkManagerLobby
        GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;

    }

    public void PositionPlayer(NetworkConnection conn, GameObject player)
    {

        player.transform.position = spawnPoints[nextIndex].position;
        player.transform.rotation = spawnPoints[nextIndex].transform.rotation;


        NetworkServer.Spawn(player, conn);

        nextIndex++;

    }
}
