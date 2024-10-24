using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    public int health = 100;
    private int Health {  get { return health; }  set { health = value; } }


    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            PlayerManager.instance.StartRespawn(gameObject);
            Debug.Log("Player Died");
        }
    }
}
