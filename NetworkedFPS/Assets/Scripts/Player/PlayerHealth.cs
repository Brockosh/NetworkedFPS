using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour, IDamageable
{
    [SyncVar]
    public int health = 100;
    private int Health {  get { return health; }  set { health = value; } }

    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            PlayerManager.instance.PrepareRespawn(connectionToClient, GetComponent<NetworkIdentity>().netId);
            Debug.Log("Player Died");
            health = 100;
        }
    }

    public void ResetHealth()
    {
        health = 100;
        Debug.LogWarning("Health Reset");
    }
}
