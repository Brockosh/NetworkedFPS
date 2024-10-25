using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
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

    public void StartRespawn(GameObject player)
    {
        StartCoroutine(RespawnPlayer(player));
    }

    private IEnumerator RespawnPlayer(GameObject player)
    {
        player.SetActive(false);
        player.GetComponent<PlayerHealth>().health = 100;
        player.transform.position = spawner.FindSpawnPosition();
        yield return new WaitForSeconds(respawnDelay);
        player.SetActive(true);
        
    }

}
