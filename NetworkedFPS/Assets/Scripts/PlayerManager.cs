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

    //public void StartRespawn(GameObject player)
    //{
    //    StartCoroutine(RespawnPlayer(player));
    //}

    [TargetRpc]
    public void RespawnPlayer(NetworkConnectionToClient conn)
    {


        //This is actually the Player manager game object
        //Need to convert from the conn to the gameobject it is associated with, then call the right things.
        gameObject.SetActive(false);
        gameObject.GetComponent<PlayerHealth>().health = 100;
        gameObject.transform.position = spawner.FindSpawnPosition();
        gameObject.SetActive(true);



        //player.SetActive(false);
        //player.GetComponent<PlayerHealth>().health = 100;
        //player.transform.position = spawner.FindSpawnPosition();
        ////yield return new WaitForSeconds(respawnDelay);
        //player.SetActive(true);
        
    }

}
